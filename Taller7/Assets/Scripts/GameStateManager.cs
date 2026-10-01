using System;
using UnityEngine;

public enum GameState
{
    Exploring,
    ShowingMessage,
    Inspecting,
    PlayingCutscene
    // Dialogue -> se agregará cuando exista el sistema de diálogos
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public GameState CurrentState { get; private set; } = GameState.Exploring;

    public event Action<GameState, GameState> OnStateChanged; // (anterior, nuevo)

    void Awake() => Instance = this;

    public void SetState(GameState newState)
    {
        if (newState == CurrentState) return;
        var previous = CurrentState;
        CurrentState = newState;
        OnStateChanged?.Invoke(previous, newState);
    }
}