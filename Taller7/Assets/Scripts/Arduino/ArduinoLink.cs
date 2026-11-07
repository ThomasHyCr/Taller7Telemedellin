using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text;
using System.Threading;
using UnityEngine;

// Requiere: Edit > Project Settings > Player > Other Settings > Api Compatibility Level = ".NET Framework"
[DefaultExecutionOrder(-100)] // se ejecuta antes que el resto, para que los pulsadores se lean en el mismo frame
public class ArduinoLink : MonoBehaviour
{
    public static ArduinoLink Instance;

    [Header("Conexión")]
    [Tooltip("Desmárcalo para jugar solo con teclado, sin el circuito.")]
    [SerializeField] private bool enableHardware = true;
    [Tooltip("Ej.: COM5. Míralo en el IDE de Arduino (Herramientas > Puerto) o en el Administrador de dispositivos.")]
    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;

    private const int ButtonCount = 5; // W A S D J

    private SerialPort port;
    private Thread readThread;
    private volatile bool running;
    private readonly ConcurrentQueue<string> incoming = new();

    private readonly bool[] held = new bool[ButtonCount];
    private readonly int[] downFrame = new int[ButtonCount];

    /// <summary>La placa avisa que arrancó (o se reinició). Hay que reenviarle el estado de los LED.</summary>
    public event Action OnReady;

    public bool IsConnected => port != null && port.IsOpen;

    void Awake()
    {
        Instance = this;
        Application.runInBackground = true; // los pulsadores deben seguir funcionando aunque la ventana pierda el foco

        for (int i = 0; i < ButtonCount; i++) downFrame[i] = -1;
    }

    void Start()
    {
        if (enableHardware) Connect();
    }

    void Update()
    {
        while (incoming.TryDequeue(out string line))
            HandleLine(line);
    }

    void OnDestroy() => Close();
    void OnApplicationQuit() => Close();

    // ---------- API pública ----------

    /// <summary>Pulsador mantenido (índice 0..4 = W, A, S, D, J).</summary>
    public bool GetButton(int index) => held[index];

    /// <summary>Pulsador que se presionó en este frame.</summary>
    public bool GetButtonDown(int index) => downFrame[index] == Time.frameCount;

    public void SendLine(string text)
    {
        if (!IsConnected) return;

        try { port.Write(text + "\n"); }
        catch (Exception e) { Debug.LogWarning($"[Arduino] Error al enviar: {e.Message}"); }
    }

    // ---------- Conexión ----------

    void Connect()
    {
        try
        {
            port = new SerialPort(portName, baudRate) { NewLine = "\n", WriteTimeout = 200 };
            port.Open(); // la Mega se reinicia al abrir el puerto: es normal, tarda ~2 s en avisar con "READY"

            running = true;
            readThread = new Thread(ReadLoop) { IsBackground = true };
            readThread.Start();

            Debug.Log($"[Arduino] Puerto {portName} abierto.");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Arduino] No se pudo abrir {portName}: {e.Message}. " +
                             $"Puertos disponibles: {string.Join(", ", SerialPort.GetPortNames())}");
            port = null;
        }
    }

    void Close()
    {
        running = false;

        if (readThread != null && readThread.IsAlive)
            readThread.Join(300);

        if (port != null && port.IsOpen)
            port.Close();

        port = null;
    }

    // Hilo aparte: lee lo que llega y arma las líneas, sin bloquear el juego
    void ReadLoop()
    {
        var lineBuffer = new StringBuilder();

        while (running)
        {
            try
            {
                string chunk = port.ReadExisting();

                if (string.IsNullOrEmpty(chunk))
                {
                    Thread.Sleep(2);
                    continue;
                }

                foreach (char c in chunk)
                {
                    if (c == '\n')
                    {
                        incoming.Enqueue(lineBuffer.ToString().Trim());
                        lineBuffer.Clear();
                    }
                    else
                    {
                        lineBuffer.Append(c);
                    }
                }
            }
            catch (Exception e)
            {
                if (running) Debug.LogWarning($"[Arduino] Conexión perdida: {e.Message}");
                running = false;
            }
        }
    }

    // ---------- Mensajes de la placa ----------

    void HandleLine(string line)
    {
        if (line == "READY")
        {
            OnReady?.Invoke();
            return;
        }

        // "B:xxxxx" -> estado de los 5 pulsadores
        if (line.Length == 2 + ButtonCount && line.StartsWith("B:"))
        {
            for (int i = 0; i < ButtonCount; i++)
            {
                bool pressed = line[2 + i] == '1';

                if (pressed && !held[i])
                    downFrame[i] = Time.frameCount;

                held[i] = pressed;
            }
        }
    }
}