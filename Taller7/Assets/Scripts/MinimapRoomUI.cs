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

    public void Refresh(RoomNode node)
    {
        background.color = node.State switch
        {
            RoomState.Unvisited => unvisitedColor,
            RoomState.Discovered => discoveredColor,
            RoomState.Visited => visitedColor,
            RoomState.Current => currentColor,
            _ => unvisitedColor
        };

        if (icon != null)
            icon.enabled = node.State != RoomState.Unvisited;
    }
}