using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialRoomProgressTracker : MonoBehaviour
{
    public static SpecialRoomProgressTracker Instance;

    [Header("Las 4 salas especiales originales (incluye la que se reactiva)")]
    [SerializeField] private List<Vector2Int> originalSpecialRooms;

    [Header("Imagen de fondo del final (opcional)")]
    [SerializeField] private GameObject finalBackground; // debe estar DESACTIVADO en el editor

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

    [Header("Mensaje final (tras la segunda animación del aeropuerto). Al pasarlo se reinicia la escena.")]
    [SerializeField, TextArea(2, 4)] private string[] finalLines =
    {
        "Has completado todos los reportajes.",
        "Gracias por jugar. La experiencia volverá a empezar."
    };

    private bool hasReactivated = false;

    void Awake() => Instance = this;

    /// <summary>
    /// Llamar cuando termina un reportaje, con el estado ya en Exploring.
    /// Muestra el mensaje del reportaje y, según el caso:
    ///   - si con este se completan los 4: reactiva el Aeropuerto y muestra el mensaje de los 4 completados;
    ///   - si es la segunda animación del Aeropuerto (la última): muestra el mensaje final y reinicia la escena.
    /// </summary>
    public void OnReportFinished(Vector2Int solvedRoomPosition, string[] afterReportLines)
    {
        var lines = new List<string>();
        if (afterReportLines != null) lines.AddRange(afterReportLines);

        // Se calcula antes de tocar hasReactivated: la última animación es la del Aeropuerto
        // cuando ya estaba reactivado (es decir, su segunda vez).
        bool isFinalReport = hasReactivated && solvedRoomPosition == reactivatedRoomPosition;
        bool completesTheFour = !hasReactivated && AllOriginalSolved();

        if (completesTheFour)
        {
            hasReactivated = true;
            ReactivateRoom();
            lines.AddRange(allReportsCompletedLines);
        }

        if (isFinalReport)
        {
            lines.AddRange(finalLines);

            if (finalBackground != null)
                finalBackground.SetActive(true); // aparece apenas termina la animación; la recarga de la escena lo vuelve a ocultar
        }

        // Si el último reportaje de los 4 fue justo el de la sala que se reactiva,
        // te devuelve a la casilla anterior cuando termines de leer los mensajes.
        bool mustReturnToPrevious = completesTheFour && solvedRoomPosition == reactivatedRoomPosition;

        DialogueManager.Instance.Show(lines, () =>
        {
            if (isFinalReport)
            {
                SceneReloader.Reload();
                return;
            }

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