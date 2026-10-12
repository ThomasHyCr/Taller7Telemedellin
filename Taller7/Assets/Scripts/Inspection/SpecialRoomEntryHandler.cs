using UnityEngine;

public class SpecialRoomEntryHandler : MonoBehaviour
{
    [SerializeField, TextArea(2, 4)] private string[] entryLines =
    {
        "Aquí hay algo que se puede inspeccionar."
    };

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

        DialogueManager.Instance.Show(entryLines, () =>
        {
            GameStateManager.Instance.SetState(GameState.Inspecting);
            SpecialRoomRegistry.Instance.ActivateRoom(room.GridPosition);
        });
    }
}