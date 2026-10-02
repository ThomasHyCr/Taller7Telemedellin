using UnityEngine;
using UnityEngine.Video;

public class InspectableObject : MonoBehaviour, IInspectable
{
    [SerializeField] private bool isCorrectObject;
    [SerializeField] private GameObject highlightVisual; // outline/glow, desactivado por defecto
    [SerializeField] private VideoClip cutsceneClip;      // solo se usa si isCorrectObject = true

    public RectTransform RectTransform => transform as RectTransform;

    public void SetHighlighted(bool value)
    {
        if (highlightVisual != null) highlightVisual.SetActive(value);
    }

    public void Interact()
    {
        if (!isCorrectObject)
        {
            // TODO: cuando exista el sistema de diálogos, mostrar algo tipo "no parece nada interesante"
            Debug.Log("Objeto incorrecto (sin efecto por ahora).");
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

        SpecialRoomProgressTracker.Instance.NotifyRoomSolved(room.GridPosition);
    }
}