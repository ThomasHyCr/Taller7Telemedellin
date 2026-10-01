using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecialRoomEntry
{
    public Vector2Int RoomGridPosition;
    public RectTransform InspectionPanel;           // también define los límites de movimiento del cursor
    public List<MonoBehaviour> InspectableNodes;     // cada uno DEBE implementar IInspectable (InspectableObject o ReturnButtonNode)
    public Vector2 CursorStartPosition = Vector2.zero; // posición local dentro del panel donde aparece el cursor al activarse
}

public class SpecialRoomRegistry : MonoBehaviour
{
    public static SpecialRoomRegistry Instance;

    [SerializeField] private List<SpecialRoomEntry> specialRooms;
    [SerializeField] private InspectionCursorController cursorController;

    void Awake() => Instance = this;

    public void ActivateRoom(Vector2Int gridPosition)
    {
        var entry = specialRooms.Find(e => e.RoomGridPosition == gridPosition);
        if (entry == null)
        {
            Debug.LogWarning($"No hay InspectionPanel configurado para la sala especial {gridPosition}");
            return;
        }

        entry.InspectionPanel.gameObject.SetActive(true);

        var nodes = new List<IInspectable>();
        foreach (var mb in entry.InspectableNodes)
        {
            if (mb is IInspectable inspectable) nodes.Add(inspectable);
            else Debug.LogWarning($"{mb.name} está en InspectableNodes pero no implementa IInspectable");
        }

        cursorController.Activate(nodes, entry.InspectionPanel, entry.CursorStartPosition);
    }

    public void DeactivateRoom(Vector2Int gridPosition)
    {
        var entry = specialRooms.Find(e => e.RoomGridPosition == gridPosition);
        if (entry != null) entry.InspectionPanel.gameObject.SetActive(false);
        cursorController.Deactivate();
    }
}