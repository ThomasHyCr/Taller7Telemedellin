using UnityEngine;

public class PlayerRoomInput : MonoBehaviour
{
    void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Exploring) return;

        if (GameInput.GetKeyDown(KeyCode.W)) DungeonManager.Instance.TryMove(Direction.Up);
        else if (GameInput.GetKeyDown(KeyCode.S)) DungeonManager.Instance.TryMove(Direction.Down);
        else if (GameInput.GetKeyDown(KeyCode.A)) DungeonManager.Instance.TryMove(Direction.Left);
        else if (GameInput.GetKeyDown(KeyCode.D)) DungeonManager.Instance.TryMove(Direction.Right);
    }
}