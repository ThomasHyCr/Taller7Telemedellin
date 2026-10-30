using UnityEngine;
using UnityEngine.Video;

public class SceneIntroMessage : MonoBehaviour
{
    [SerializeField] private VideoClip introClip;

    [SerializeField, TextArea(2, 4)] private string[] introLines =
    {
        "Usa W, A, S y D para moverte entre los lugares del mapa.",
        "Pulsa J para avanzar los mensajes e interactuar con los objetos.",
        "Los lugares que brillan en amarillo tienen algo que inspeccionar."
    };

    void Start()
    {
        CutscenePlayer.Instance.PlayInspectionCutscene(introClip, OnIntroFinished);
    }

    void OnIntroFinished()
    {
        GameStateManager.Instance.SetState(GameState.Exploring);
        DialogueManager.Instance.Show(introLines);
    }
}