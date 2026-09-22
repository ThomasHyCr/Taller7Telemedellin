using System.Collections.Generic;
using UnityEngine;

public enum Direction { Up, Down, Left, Right }
public enum RoomState { Unvisited, Discovered, Visited, Current }

[System.Serializable]
public class RoomNode
{
    public Vector2Int GridPosition;
    public RoomState State = RoomState.Unvisited;
    public Dictionary<Direction, RoomNode> Connections = new();

    public Sprite RoomImage; // Imagen fija que se muestra al estar en esta sala

    public bool HasConnection(Direction dir) => Connections.ContainsKey(dir);

    public static Vector2Int DirectionToOffset(Direction dir) => dir switch
    {
        Direction.Up => Vector2Int.up,
        Direction.Down => Vector2Int.down,
        Direction.Left => Vector2Int.left,
        Direction.Right => Vector2Int.right,
        _ => Vector2Int.zero
    };
}