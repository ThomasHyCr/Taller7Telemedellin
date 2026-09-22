using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Coloca este script dentro de una carpeta "Editor" (ej: Assets/Editor/).
/// Reconstruye el asset "MapLayout" (DungeonLayoutSO) con los datos
/// rescatados del YAML dañado, asignados directamente por código
/// en vez de a mano en el Inspector.
///
/// Las imágenes (RoomImage) de cada sala NO se pueden reconstruir por código
/// (no tenemos esa referencia guardada) — deberás asignarlas manualmente
/// en el Inspector una vez creado el asset.
/// </summary>
public static class DungeonLayoutRebuilder
{
    private const string AssetPath = "Assets/Scripts/MapLayout.asset"; // ajusta la carpeta si es otra

    [MenuItem("Assets/Dungeon Layout/Rebuild From Salvaged Data")]
    private static void Rebuild()
    {
        DungeonLayoutSO layout = AssetDatabase.LoadAssetAtPath<DungeonLayoutSO>(AssetPath);
        bool isNew = layout == null;

        if (isNew)
        {
            layout = ScriptableObject.CreateInstance<DungeonLayoutSO>();
        }

        layout.Rooms = new List<DungeonLayoutSO.RoomData>
        {
            Room(0, 0),   Room(-3, -4), Room(3, -3),  Room(-3, 4),  Room(3, 4),
            Room(-2, -4), Room(-2, -2), Room(-2, -1), Room(-2, 1),  Room(-2, 3),
            Room(-3, 3),  Room(-1, -1), Room(-1, 0),  Room(1, -2),  Room(1, -1),
            Room(1, 0),   Room(1, 1),   Room(1, 2),   Room(1, 3),   Room(2, -3),
            Room(2, -2),  Room(3, -2),  Room(3, 1),   Room(3, 3),
        };

        layout.Connections = new List<RoomConnectionData>
        {
            Conn(-3, -4, Direction.Right, -2, -4),
            Conn(-2, -4, Direction.Up,    -2, -2),
            Conn(-2, -2, Direction.Up,    -2, -1),
            Conn(-2, -1, Direction.Up,    -2,  1),
            Conn(-2, -1, Direction.Right, -1, -1),
            Conn(-2,  1, Direction.Up,    -2,  3),
            Conn(-2,  3, Direction.Left,  -3,  3),
            Conn(-3,  3, Direction.Left,  -3,  4),
            Conn(-1, -1, Direction.Up,    -1,  0),
            Conn(-1, -1, Direction.Right,  1, -1),
            Conn(-1,  0, Direction.Right,  0,  0),
            Conn( 0,  0, Direction.Right,  1,  0),
            Conn( 1,  0, Direction.Up,     1,  1),
            Conn( 1,  1, Direction.Up,     1,  2),
            Conn( 1,  2, Direction.Up,     1,  3),
            Conn( 1,  3, Direction.Right,  3,  3),
            Conn( 3,  3, Direction.Up,     3,  4),
            Conn( 1,  1, Direction.Right, -2,  1),
            Conn( 1,  1, Direction.Left,   3,  1),
            Conn( 1, -1, Direction.Up,     0,  1), // ⚠️ (0,1) no existe en Rooms — revisar
            Conn( 1, -1, Direction.Down,   1, -2),
            Conn( 1, -2, Direction.Right,  2, -2),
            Conn( 2, -2, Direction.Down,   2, -3),
            Conn( 2, -3, Direction.Right,  3, -3),
            Conn( 2, -2, Direction.Right,  3, -2),
            Conn( 3, -2, Direction.Up,     3,  1),
        };

        layout.StartingRoom = new Vector2Int(3, -3);

        // Validación: avisa si alguna conexión apunta a una sala inexistente
        var roomSet = new HashSet<Vector2Int>();
        foreach (var r in layout.Rooms) roomSet.Add(r.GridPosition);

        foreach (var c in layout.Connections)
        {
            if (!roomSet.Contains(c.From))
                Debug.LogWarning($"Conexión con 'From' inexistente en Rooms: {c.From}");
            if (!roomSet.Contains(c.To))
                Debug.LogWarning($"Conexión con 'To' inexistente en Rooms: {c.To} (revisa si falta agregar esa sala o si es un typo)");
        }

        if (isNew)
        {
            AssetDatabase.CreateAsset(layout, AssetPath);
        }

        EditorUtility.SetDirty(layout);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = layout;
        Debug.Log($"DungeonLayoutSO reconstruido en: {AssetPath}. Revisa la consola por warnings de datos inconsistentes, y asigna manualmente los RoomImage de cada sala.");
    }

    private static DungeonLayoutSO.RoomData Room(int x, int y) =>
        new DungeonLayoutSO.RoomData { GridPosition = new Vector2Int(x, y), RoomImage = null };

    private static RoomConnectionData Conn(int fx, int fy, Direction dir, int tx, int ty) =>
        new RoomConnectionData { From = new Vector2Int(fx, fy), Direction = dir, To = new Vector2Int(tx, ty) };
}
