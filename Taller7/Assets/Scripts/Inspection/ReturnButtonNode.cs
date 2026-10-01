using UnityEngine;

public class ReturnButtonNode : MonoBehaviour, IInspectable
{
    [SerializeField] private GameObject highlightVisual;

    public RectTransform RectTransform => transform as RectTransform;

    public void SetHighlighted(bool value)
    {
        if (highlightVisual != null) highlightVisual.SetActive(value);
    }

    public void Interact()
    {
        var room = DungeonManager.Instance.CurrentRoom;
        SpecialRoomRegistry.Instance.DeactivateRoom(room.GridPosition);
        GameStateManager.Instance.SetState(GameState.Exploring);
        DungeonManager.Instance.ReturnToPreviousRoom();
    }
}