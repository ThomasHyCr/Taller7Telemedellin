using UnityEngine;
using UnityEngine.UI;

public class MinimapRoomUI : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image icon; // opcional: calavera, cofre, "?", etc.

    [SerializeField] private Color unvisitedColor = new(0.15f, 0.15f, 0.15f);
    [SerializeField] private Color discoveredColor = new(0.4f, 0.4f, 0.4f);
    [SerializeField] private Color visitedColor = Color.gray;
    [SerializeField] private Color currentColor = Color.white;

    [Header("Sala especial sin resolver")]
    [SerializeField] private Color specialPendingColor = Color.yellow;
    [SerializeField] private float oscillationSpeed = 2f;

    private Color baseColor;
    private bool isSpecialPending;

    public void Refresh(RoomNode node)
    {
        baseColor = node.State switch
        {
            RoomState.Unvisited => unvisitedColor,
            RoomState.Discovered => discoveredColor,
            RoomState.Visited => visitedColor,
            RoomState.Current => currentColor,
            _ => unvisitedColor
        };

        // Solo oscila si ya fue descubierta (no revela salas especiales ocultas en la niebla)
        isSpecialPending = node.IsSpecial && !node.SpecialSolved;

        if (!isSpecialPending)
            background.color = baseColor;

        if (icon != null)
            icon.enabled = node.State != RoomState.Unvisited;
    }

    void Update()
    {
        if (!isSpecialPending) return;

        float t = Mathf.PingPong(Time.time * oscillationSpeed, 1f);
        background.color = Color.Lerp(baseColor, specialPendingColor, t);
    }
}