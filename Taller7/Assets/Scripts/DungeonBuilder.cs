using UnityEngine;

public class DungeonBuilder : MonoBehaviour
{
    [SerializeField] private DungeonLayoutSO layout;

    void Awake()
    {
        BuildGraph();
    }

    void BuildGraph()
    {
        var manager = DungeonManager.Instance;

        // 1. Crear todos los nodos
        foreach (var roomData in layout.Rooms)
        {
            var node = new RoomNode
            {
                GridPosition = roomData.GridPosition,
                RoomImage = roomData.RoomImage
            };
            manager.Rooms[roomData.GridPosition] = node;
        }

        // 2. Conectarlos (en ambas direcciones automáticamente)
        foreach (var conn in layout.Connections)
        {
            var from = manager.Rooms[conn.From];
            var to = manager.Rooms[conn.To];

            from.Connections[conn.Direction] = to;
            to.Connections[Opposite(conn.Direction)] = from;
        }

        // 3. Setear sala inicial
        manager.SetStartingRoom(manager.Rooms[layout.StartingRoom]);
    }

    Direction Opposite(Direction dir) => dir switch
    {
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        _ => dir
    };
}