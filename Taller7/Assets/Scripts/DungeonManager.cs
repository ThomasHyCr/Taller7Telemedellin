using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    public Dictionary<Vector2Int, RoomNode> Rooms = new();
    public RoomNode CurrentRoom { get; private set; }

    public event Action<RoomNode> OnCurrentRoomChanged;
    public event Action<RoomNode> OnRoomStateChanged;

    void Awake() => Instance = this;

    /// <summary>
    /// Debe llamarse una vez al iniciar el nivel, luego de construir el grafo de salas.
    /// </summary>
    public void SetStartingRoom(RoomNode startRoom)
    {
        CurrentRoom = startRoom;
        CurrentRoom.State = RoomState.Current;
        OnRoomStateChanged?.Invoke(CurrentRoom);
        OnCurrentRoomChanged?.Invoke(CurrentRoom);

        // Descubre las vecinas iniciales
        foreach (var kv in CurrentRoom.Connections)
        {
            if (kv.Value.State == RoomState.Unvisited)
            {
                kv.Value.State = RoomState.Discovered;
                OnRoomStateChanged?.Invoke(kv.Value);
            }
        }
    }

    public bool TryMove(Direction dir)
    {
        if (CurrentRoom == null || !CurrentRoom.HasConnection(dir))
            return false;

        RoomNode target = CurrentRoom.Connections[dir];

        CurrentRoom.State = RoomState.Visited;
        OnRoomStateChanged?.Invoke(CurrentRoom);

        CurrentRoom = target;
        CurrentRoom.State = RoomState.Current;
        OnRoomStateChanged?.Invoke(CurrentRoom);

        // Descubre vecinas nuevas (efecto "niebla de guerra")
        foreach (var kv in CurrentRoom.Connections)
        {
            if (kv.Value.State == RoomState.Unvisited)
            {
                kv.Value.State = RoomState.Discovered;
                OnRoomStateChanged?.Invoke(kv.Value);
            }
        }

        OnCurrentRoomChanged?.Invoke(CurrentRoom);
        return true;
    }
}