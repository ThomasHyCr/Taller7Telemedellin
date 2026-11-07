using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Envía a la placa qué debe hacer cada LED:
///   - sala actual                       -> encendido fijo
///   - sala especial sin resolver        -> parpadeando
///   - cualquier otra                    -> apagado
///
/// Qué sala representa cada LED se edita en el Inspector (lista "Leds").
/// El LED n está en el pin D(21+n): LED 1-20 = D22-D41 (blancos), LED 21-24 = D42-D45 (amarillos).
/// </summary>
public class ArduinoLedSync : MonoBehaviour
{
    private const int LedCount = 24;
    private const int WhiteLeds = 20; // LED 1-20 blancos; LED 21-24 amarillos
    private const int FirstPin = 22;

    [System.Serializable]
    public class LedSlot
    {
        [Tooltip("Se rellena sola: número de LED, pin y color.")]
        public string label;
        [Tooltip("Coordenadas de la sala que representa este LED.")]
        public Vector2Int room;
        [Tooltip("Desmárcalo para dejar este LED siempre apagado (repuesto, dañado, sin usar).")]
        public bool used = true;
    }

    // Punto de partida: el orden que usaba el cableado automático (de abajo hacia arriba y de izquierda a derecha)
    private static readonly Vector2Int[] DefaultRooms =
    {
        // LED 1-20 (blancos, salas normales)
        new(-2, -4), new(2, -3), new(3, -3), new(-2, -2), new(1, -2), new(2, -2), new(3, -2),
        new(-2, -1), new(-1, -1), new(1, -1), new(-1, 0), new(1, 0), new(-2, 1), new(1, 1),
        new(3, 1), new(1, 2), new(-3, 3), new(-2, 3), new(1, 3), new(3, 3),
        // LED 21-24 (amarillos, salas especiales)
        new(-3, -4), new(0, 0), new(-3, 4), new(3, 4)
    };

    [Header("Mapa de LED (el elemento 0 es el LED 1, y así sucesivamente)")]
    [SerializeField] private List<LedSlot> leds = CreateDefaults();

    [Tooltip("Cada cuántos segundos se reenvía el estado aunque no haya cambios (cubre reinicios de la placa).")]
    [SerializeField] private float resendInterval = 2f;

    private bool validated;
    private bool dirty = true;
    private float lastSent = float.NegativeInfinity;

    static List<LedSlot> CreateDefaults()
    {
        var list = new List<LedSlot>();
        for (int i = 0; i < LedCount; i++)
            list.Add(new LedSlot { room = DefaultRooms[i], used = true });
        return list;
    }

    // Mantiene la lista en 24 elementos y con las etiquetas al día
    void OnValidate()
    {
        while (leds.Count < LedCount) leds.Add(new LedSlot());
        while (leds.Count > LedCount) leds.RemoveAt(leds.Count - 1);

        for (int i = 0; i < leds.Count; i++)
            leds[i].label = $"LED {i + 1} · D{FirstPin + i} · {(i < WhiteLeds ? "blanco" : "amarillo")}";
    }

    void Start()
    {
        DungeonManager.Instance.OnRoomStateChanged += OnRoomChanged;

        if (ArduinoLink.Instance != null)
            ArduinoLink.Instance.OnReady += OnArduinoReady;
    }

    void OnDestroy()
    {
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.OnRoomStateChanged -= OnRoomChanged;

        if (ArduinoLink.Instance != null)
            ArduinoLink.Instance.OnReady -= OnArduinoReady;
    }

    void OnRoomChanged(RoomNode room) => dirty = true;
    void OnArduinoReady() => dirty = true;

    void LateUpdate()
    {
        var dungeon = DungeonManager.Instance;
        if (dungeon.CurrentRoom == null || dungeon.Rooms.Count == 0) return;

        // El mapa de salas se arma en DungeonBuilder, así que la revisión se hace la primera vez que existe
        if (!validated)
        {
            ValidateAndLogMapping();
            validated = true;
        }

        if (GameStateManager.Instance.CurrentState == GameState.Title) return; // pausado hasta cerrar el panel de inicio

        var link = ArduinoLink.Instance;
        if (link == null || !link.IsConnected) return;

        // Solo se envía si algo cambió (varios cambios en un mismo frame = un solo mensaje) o toca el reenvío periódico
        if (!dirty && Time.unscaledTime - lastSent < resendInterval) return;

        link.SendLine(BuildSnapshot());
        dirty = false;
        lastSent = Time.unscaledTime;
    }

    // ---------- Revisión del mapa de LED ----------

    void ValidateAndLogMapping()
    {
        var rooms = DungeonManager.Instance.Rooms;
        var assigned = new HashSet<Vector2Int>();
        var table = new StringBuilder("[Arduino] Tabla de cableado (LED -> sala):\n");

        for (int i = 0; i < leds.Count && i < LedCount; i++)
        {
            var slot = leds[i];
            string header = $"LED {i + 1} (D{FirstPin + i}, {(i < WhiteLeds ? "blanco" : "amarillo")})";

            if (!slot.used)
            {
                table.AppendLine($"{header} -> sin usar");
                continue;
            }

            if (!rooms.ContainsKey(slot.room))
            {
                Debug.LogWarning($"[Arduino] {header} apunta a {slot.room}, pero esa sala no existe en el mapa.");
                table.AppendLine($"{header} -> {slot.room} (NO EXISTE)");
                continue;
            }

            if (!assigned.Add(slot.room))
                Debug.LogWarning($"[Arduino] La sala {slot.room} tiene más de un LED asignado.");

            table.AppendLine($"{header} -> sala {slot.room}");
        }

        foreach (var position in rooms.Keys)
        {
            if (!assigned.Contains(position))
                Debug.LogWarning($"[Arduino] La sala {position} no tiene ningún LED asignado.");
        }

        Debug.Log(table.ToString());
    }

    // ---------- Mensaje para la placa ----------

    string BuildSnapshot()
    {
        var dungeon = DungeonManager.Instance;
        var current = dungeon.CurrentRoom;
        var chars = new char[LedCount];

        for (int i = 0; i < LedCount; i++)
        {
            chars[i] = '0';

            if (i >= leds.Count || !leds[i].used) continue;
            if (!dungeon.Rooms.TryGetValue(leds[i].room, out var room)) continue;

            bool pendingSpecial = room.IsSpecial && !room.SpecialSolved;

            if (room == current) chars[i] = '1';        // donde estás: fijo
            else if (pendingSpecial) chars[i] = '2';    // reportaje pendiente: parpadea
        }

        return "L:" + new string(chars);
    }
}