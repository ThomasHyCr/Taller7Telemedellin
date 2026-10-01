using UnityEngine;

public class SpecialRoomEntryHandler : MonoBehaviour
{
    [SerializeField] private SimpleMessagePrompt messagePrompt;

    void Start()
    {
        DungeonManager.Instance.OnCurrentRoomChanged += HandleRoomChanged;
    }

    void OnDestroy()
    {
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.OnCurrentRoomChanged -= HandleRoomChanged;
    }

    void HandleRoomChanged(RoomNode room)
    {
        if (!room.IsSpecial || room.SpecialSolved) return;

        GameStateManager.Instance.SetState(GameState.ShowingMessage);
        messagePrompt.ShowMessage("Aquí hay algo que se puede inspeccionar", () =>
        {
            GameStateManager.Instance.SetState(GameState.Inspecting);
            SpecialRoomRegistry.Instance.ActivateRoom(room.GridPosition);
        });
    }
}