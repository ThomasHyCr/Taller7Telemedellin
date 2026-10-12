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

    [Header("Mensaje al completar los 4 primeros reportajes")]
    [SerializeField, TextArea(2, 4)] private string[] allReportsCompletedLines =
    {
        "Se completaron los cuatro primeros reportajes.",
        "Algo ha cambiado en el aeropuerto."
    };

    private bool hasReactivated = false;

    void Awake() => Instance = this;

    /// <summary>
    /// Llamar cuando termina un reportaje, con el estado ya en Exploring.
    /// Muestra el mensaje del reportaje y, si con este se completan los 4,
    /// reactiva el Aeropuerto y muestra también el mensaje de los 4 completados.
    /// </summary>
    public void OnReportFinished(Vector2Int solvedRoomPosition, string[] afterReportLines)
    {
        var lines = new List<string>();
        if (afterReportLines != null) lines.AddRange(afterReportLines);

        bool completesTheFour = !hasReactivated && AllOriginalSolved();

        if (completesTheFour)
        {
            hasReactivated = true;
            ReactivateRoom();
            lines.AddRange(allReportsCompletedLines);
        }

        // Si el último reportaje fue justo el de la sala que se reactiva,
        // te devuelve a la casilla anterior cuando termines de leer los mensajes.
        bool mustReturnToPrevious = completesTheFour && solvedRoomPosition == reactivatedRoomPosition;

        DialogueManager.Instance.Show(lines, () =>
        {
            if (mustReturnToPrevious)
                DungeonManager.Instance.ReturnToPreviousRoom();
        });
    }

    bool AllOriginalSolved()
    {
        return originalSpecialRooms.All(pos => DungeonManager.Instance.Rooms[pos].SpecialSolved);
    }

    void ReactivateRoom()
    {
        var room = DungeonManager.Instance.Rooms[reactivatedRoomPosition];
        room.SpecialSolved = false; // vuelve a ser una sala especial sin resolver
        DungeonManager.Instance.RefreshRoomVisual(room); // vuelve a oscilar en el minimapa

        SpecialRoomRegistry.Instance.ReplaceRoomContent(
            reactivatedRoomPosition, reactivatedPanel, reactivatedNodes, reactivatedCursorStart);
    }
}