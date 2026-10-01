using UnityEngine;

public class ReturnButtonNode : MonoBehaviour, IInspectableNode
{
    [SerializeField] private MonoBehaviour up, down, left, right;
    [SerializeField] private GameObject highlightVisual;

    public IInspectableNode Up => up as IInspectableNode;
    public IInspectableNode Down => down as IInspectableNode;
    public IInspectableNode Left => left as IInspectableNode;
    public IInspectableNode Right => right as IInspectableNode;
    public Transform HighlightTarget => transform;

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