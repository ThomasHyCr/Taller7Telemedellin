using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLayout", menuName = "Dungeon/Layout")]
public class DungeonLayoutSO : ScriptableObject
{
    [System.Serializable]
    public class RoomData
    {
        public Vector2Int GridPosition;
        public Sprite RoomImage;
    }

    public List<RoomData> Rooms = new();
    public List<RoomConnectionData> Connections = new();
    public Vector2Int StartingRoom;
}