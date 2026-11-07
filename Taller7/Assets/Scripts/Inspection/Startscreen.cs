using System;
using UnityEngine;

/// <summary>
/// Panel de inicio. Mientras está abierto, el juego está en el estado Title (todo lo demás en pausa).
/// Al pulsar J se cierra, se pasa a Exploring y se avisa con el evento Closed para que arranque el flujo normal.
/// </summary>
public class StartScreen : MonoBehaviour
{
    public static StartScreen Instance;

    [SerializeField] private GameObject panel; // el panel de inicio (hijo del Canvas)

    /// <summary>Se dispara justo después de cerrar el panel y pasar a Exploring.</summary>
    public event Action Closed;

    void Awake()
    {
        Instance = this;
        panel.SetActive(true); // aparece siempre al arrancar, esté como esté en el editor
    }

    void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Title) return;
        if (!Input.GetKeyDown(KeyCode.J)) return;

        Close();
    }

    void Close()
    {
        panel.SetActive(false);
        GameStateManager.Instance.SetState(GameState.Exploring);
        Closed?.Invoke();
    }
}