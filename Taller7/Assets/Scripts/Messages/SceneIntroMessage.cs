using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Flujo inicial: panel de inicio (J) -> animación inicial (opcional) -> mensajes del tutorial -> juego normal.
/// </summary>
public class SceneIntroMessage : MonoBehaviour
{
    [Header("Animación inicial (opcional)")]
    [SerializeField] private VideoClip introClip;

    [Header("Tutorial")]
    [SerializeField, TextArea(2, 4)] private string[] introLines =
    {
        "Usa W, A, S y D para moverte entre los lugares del mapa.",
        "Pulsa J para avanzar los mensajes e interactuar con los objetos.",
        "Los lugares que brillan en amarillo tienen algo que inspeccionar."
    };

    void Start()
    {
        if (StartScreen.Instance != null)
        {
            StartScreen.Instance.Closed += BeginIntro;
        }
        else
        {
            // Sin panel de inicio en la escena: arranca directo
            GameStateManager.Instance.SetState(GameState.Exploring);
            BeginIntro();
        }
    }

    void OnDestroy()
    {
        if (StartScreen.Instance != null)
            StartScreen.Instance.Closed -= BeginIntro;
    }

    void BeginIntro()
    {
        if (introClip != null)
            CutscenePlayer.Instance.Play(introClip, OnIntroVideoFinished);
        else
            ShowTutorial();
    }

    void OnIntroVideoFinished()
    {
        // Tras un video el estado queda en PlayingCutscene. Se vuelve a Exploring antes de abrir
        // el diálogo, porque el diálogo restaura al cerrarse el estado en el que lo encontró.
        GameStateManager.Instance.SetState(GameState.Exploring);
        ShowTutorial();
    }

    void ShowTutorial()
    {
        DialogueManager.Instance.Show(introLines);
    }
}