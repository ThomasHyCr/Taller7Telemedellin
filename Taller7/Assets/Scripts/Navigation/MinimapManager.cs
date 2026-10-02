using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    [Header("Casillas")]
    [SerializeField] private RectTransform roomsContainer;
    [SerializeField] private MinimapRoomUI roomPrefab;

    [Header("Líneas de conexión")]
    [SerializeField] private RectTransform linesContainer; // debe estar ANTES que roomsContainer en la Hierarchy
    [SerializeField] private Image linePrefab;
    [SerializeField] private Color connectionLineColor = Color.red;
    [SerializeField] private float lineThickness = 4f;

    [Header("General")]
    [SerializeField] private float cellSize = 60f;

    private Dictionary<Vector2Int, MinimapRoomUI> uiRooms = new();

    void Start()
    {
        SpawnConnectionLines();

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
        var ui = Instantiate(roomPrefab, roomsContainer);
        ui.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(node.GridPosition.x, node.GridPosition.y) * cellSize;
        ui.Refresh(node);
        uiRooms[node.GridPosition] = ui;
    }

    void SpawnConnectionLines()
    {
        var drawnPairs = new HashSet<(Vector2Int, Vector2Int)>();

        foreach (var kv in DungeonManager.Instance.Rooms)
        {
            var from = kv.Value;

            foreach (var connection in from.Connections.Values)
            {
                var to = connection;

                // Normaliza el par para no dibujar la misma línea dos veces (A->B y B->A)
                var pair = NormalizePair(from.GridPosition, to.GridPosition);
                if (drawnPairs.Contains(pair)) continue;
                drawnPairs.Add(pair);

                SpawnLine(from.GridPosition, to.GridPosition);
            }
        }
    }

    (Vector2Int, Vector2Int) NormalizePair(Vector2Int a, Vector2Int b)
    {
        // Orden consistente sin importar en qué dirección se recorrió la conexión
        if (a.x != b.x) return a.x < b.x ? (a, b) : (b, a);
        return a.y < b.y ? (a, b) : (b, a);
    }

    void SpawnLine(Vector2Int fromGrid, Vector2Int toGrid)
    {
        Vector2 fromPos = new Vector2(fromGrid.x, fromGrid.y) * cellSize;
        Vector2 toPos = new Vector2(toGrid.x, toGrid.y) * cellSize;

        Vector2 delta = toPos - fromPos;
        float distance = delta.magnitude;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        var line = Instantiate(linePrefab, linesContainer);
        var rt = line.rectTransform;

        rt.anchoredPosition = fromPos;
        rt.sizeDelta = new Vector2(distance, lineThickness);
        rt.localRotation = Quaternion.Euler(0f, 0f, angle);

        line.color = connectionLineColor;
    }

    void HandleRoomStateChanged(RoomNode node)
    {
        if (uiRooms.TryGetValue(node.GridPosition, out var ui))
            ui.Refresh(node);
    }
}