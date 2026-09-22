using UnityEngine;

public class ExitIndicators : MonoBehaviour
{
    [SerializeField] private GameObject arrowUp, arrowDown, arrowLeft, arrowRight;

    void Start()
    {
        DungeonManager.Instance.OnCurrentRoomChanged += Refresh;
        Refresh(DungeonManager.Instance.CurrentRoom);
    }

    void OnDestroy()
    {
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.OnCurrentRoomChanged -= Refresh;
    }

    void Refresh(RoomNode room)
    {
        arrowUp.SetActive(room.HasConnection(Direction.Up));
        arrowDown.SetActive(room.HasConnection(Direction.Down));
        arrowLeft.SetActive(room.HasConnection(Direction.Left));
        arrowRight.SetActive(room.HasConnection(Direction.Right));
    }
}