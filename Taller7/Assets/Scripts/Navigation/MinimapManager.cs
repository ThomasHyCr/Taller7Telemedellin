using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    [SerializeField] private RectTransform container; // panel vacío del Canvas
    [SerializeField] private MinimapRoomUI roomPrefab;
    [SerializeField] private float cellSize = 60f;

    private Dictionary<Vector2Int, MinimapRoomUI> uiRooms = new();

    void Start()
    {
        foreach (var kv in DungeonManager.Instance.Rooms)
            SpawnRoomUI(kv.Value);

        DungeonManager.Instance.OnRoomStateChanged += HandleRoomStateChanged;
    }

    void OnDestroy()
    {
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.OnRoomStateChanged -= HandleRoomStateChanged;
    }

    void SpawnRoomUI(RoomNode node)
    {
        var ui = Instantiate(roomPrefab, container);
        ui.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(node.GridPosition.x, node.GridPosition.y) * cellSize;
        ui.Refresh(node);
        uiRooms[node.GridPosition] = ui;
    }

    void HandleRoomStateChanged(RoomNode node)
    {
        if (uiRooms.TryGetValue(node.GridPosition, out var ui))
            ui.Refresh(node);
    }
}