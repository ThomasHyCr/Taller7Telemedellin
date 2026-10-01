using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecialRoomEntry
{
    public Vector2Int RoomGridPosition;
    public GameObject InspectionPanel;   // contiene los InspectableObject + ReturnButtonNode de ESA sala
    public MonoBehaviour DefaultSelected; // primer nodo seleccionado (debe implementar IInspectableNode)
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

        entry.InspectionPanel.SetActive(true);
        cursorController.Activate(entry.DefaultSelected as IInspectableNode);
    }

    public void DeactivateRoom(Vector2Int gridPosition)
    {
        var entry = specialRooms.Find(e => e.RoomGridPosition == gridPosition);
        if (entry != null) entry.InspectionPanel.SetActive(false);
        cursorController.Deactivate();
    }
}