using UnityEngine;
using UnityEngine.Video;

public class InspectableObject : MonoBehaviour, IInspectable
{
    [SerializeField] private bool isCorrectObject;
    [SerializeField] private GameObject highlightVisual; // outline/glow, desactivado por defecto

    [Header("Si es un objeto incorrecto")]
    [SerializeField, TextArea(2, 4)] private string[] wrongObjectLines =
    {
        "No parece que haya nada interesante aquí."
    };

    [Header("Si es el objeto correcto")]
    [SerializeField] private VideoClip cutsceneClip;
    [SerializeField, TextArea(2, 4)] private string[] afterReportLines =
    {
        "Reportaje completado."
    };

    public RectTransform RectTransform => transform as RectTransform;

    public void SetHighlighted(bool value)
    {
        if (highlightVisual != null) highlightVisual.SetActive(value);
    }

    public void Interact()
    {
        if (!isCorrectObject)
        {
            DialogueManager.Instance.Show(wrongObjectLines);
            return;
        }

        CutscenePlayer.Instance.PlayInspectionCutscene(cutsceneClip, OnCutsceneFinished);
    }

    void OnCutsceneFinished()
    {
        var room = DungeonManager.Instance.CurrentRoom;
        room.SpecialSolved = true;
        DungeonManager.Instance.RefreshRoomVisual(room);

        SpecialRoomRegistry.Instance.DeactivateRoom(room.GridPosition);
        GameStateManager.Instance.SetState(GameState.Exploring);

        // Muestra el mensaje del reportaje (y el de los 4 completados, si corresponde)
        SpecialRoomProgressTracker.Instance.OnReportFinished(room.GridPosition, afterReportLines);
    }
}