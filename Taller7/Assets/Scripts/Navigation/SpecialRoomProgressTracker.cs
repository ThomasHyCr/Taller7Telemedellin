using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialRoomProgressTracker : MonoBehaviour
{
    public static SpecialRoomProgressTracker Instance;

    [Header("Las 4 salas especiales originales (incluye la que se reactiva)")]
    [SerializeField] private List<Vector2Int> originalSpecialRooms;

    [Header("Sala que se reactiva (Aeropuerto)")]
    [SerializeField] private Vector2Int reactivatedRoomPosition;
    [SerializeField] private RectTransform reactivatedPanel;        // InspectionPanel de la fase 2
    [SerializeField] private List<MonoBehaviour> reactivatedNodes;  // InspectableObject(s) + ReturnButtonNode de la fase 2
    [SerializeField] private Vector2 reactivatedCursorStart;

    private bool hasReactivated = false;

    void Awake() => Instance = this;

    /// <summary>
    /// Llamar justo después de marcar cualquier sala especial como resuelta.
    /// </summary>
    public void NotifyRoomSolved(Vector2Int solvedRoomPosition)
    {
        if (hasReactivated) return; // esto solo debe pasar una vez en todo el juego

        bool allSolved = originalSpecialRooms.All(pos =>
            DungeonManager.Instance.Rooms[pos].SpecialSolved);

        if (!allSolved) return;

        hasReactivated = true;

        var room = DungeonManager.Instance.Rooms[reactivatedRoomPosition];
        room.SpecialSolved = false; // "reabre" la sala como especial sin resolver
        DungeonManager.Instance.RefreshRoomVisual(room); // vuelve a oscilar en el minimapa

        SpecialRoomRegistry.Instance.ReplaceRoomContent(
            reactivatedRoomPosition, reactivatedPanel, reactivatedNodes, reactivatedCursorStart);

        // Si la sala que se acaba de resolver es justo la que se reactiva,
        // te devuelve a la sala anterior para que el flujo de entrada se dispare bien
        // la próxima vez que entres (en vez de quedarte adentro sin que se note el cambio).
        if (solvedRoomPosition == reactivatedRoomPosition)
        {
            DungeonManager.Instance.ReturnToPreviousRoom();
        }
    }
}