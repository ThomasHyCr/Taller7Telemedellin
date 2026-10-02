using UnityEngine;

public class SpecialRoomEntryHandler : MonoBehaviour
{
    [SerializeField] private SimpleMessagePrompt messagePrompt;

    void Start()
    {
        var allPrompts = FindObjectsByType<SimpleMessagePrompt>(FindObjectsSortMode.None);
        Debug.Log($"[DEBUG] Cantidad de SimpleMessagePrompt en la escena: {allPrompts.Length}");
        foreach (var p in allPrompts)
            Debug.Log($"[DEBUG] -> Encontrado en GameObject '{p.gameObject.name}' (ID={p.gameObject.GetInstanceID()})");

        Debug.Log($"[DEBUG] messagePrompt asignado en este handler apunta al script en GameObject ID={messagePrompt.gameObject.GetInstanceID()}");

        DungeonManager.Instance.OnCurrentRoomChanged += HandleRoomChanged;
    }

    void OnDestroy()
    {
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.OnCurrentRoomChanged -= HandleRoomChanged;
    }

    void HandleRoomChanged(RoomNode room)
    {
        Debug.Log($"[DEBUG] HandleRoomChanged llamado. IsSpecial={room.IsSpecial}, SpecialSolved={room.SpecialSolved}");

        if (!room.IsSpecial || room.SpecialSolved) return;

        Debug.Log("[DEBUG] Pasó el guard, voy a mostrar el mensaje");

        GameStateManager.Instance.SetState(GameState.ShowingMessage);
        messagePrompt.ShowMessage("Aquí hay algo que se puede inspeccionar", () =>
        {
            Debug.Log("[DEBUG] Callback de ShowMessage ejecutado, pasando a Inspecting");
            GameStateManager.Instance.SetState(GameState.Inspecting);
            SpecialRoomRegistry.Instance.ActivateRoom(room.GridPosition);
        });

        Debug.Log("[DEBUG] ShowMessage ya fue llamado");
    }
}