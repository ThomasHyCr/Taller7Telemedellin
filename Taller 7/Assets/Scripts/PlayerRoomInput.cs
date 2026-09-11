using UnityEngine;

public class PlayerRoomInput : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) DungeonManager.Instance.TryMove(Direction.Up);
        else if (Input.GetKeyDown(KeyCode.S)) DungeonManager.Instance.TryMove(Direction.Down);
        else if (Input.GetKeyDown(KeyCode.A)) DungeonManager.Instance.TryMove(Direction.Left);
        else if (Input.GetKeyDown(KeyCode.D)) DungeonManager.Instance.TryMove(Direction.Right);
    }
}