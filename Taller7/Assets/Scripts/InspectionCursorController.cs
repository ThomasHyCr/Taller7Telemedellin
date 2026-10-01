using System;
using UnityEngine;

public class InspectionCursorController : MonoBehaviour
{
    [SerializeField] private RectTransform cursorVisual;

    private IInspectableNode current;

    void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Inspecting) return;

        if (Input.GetKeyDown(KeyCode.W)) Move(n => n.Up);
        else if (Input.GetKeyDown(KeyCode.S)) Move(n => n.Down);
        else if (Input.GetKeyDown(KeyCode.A)) Move(n => n.Left);
        else if (Input.GetKeyDown(KeyCode.D)) Move(n => n.Right);
        else if (Input.GetKeyDown(KeyCode.J)) current?.Interact();
    }

    void Move(Func<IInspectableNode, IInspectableNode> dir)
    {
        if (current == null) return;
        var next = dir(current);
        if (next == null) return; // no hay nada en esa dirección

        current.SetHighlighted(false);
        current = next;
        current.SetHighlighted(true);
        cursorVisual.position = current.HighlightTarget.position;
    }

    public void Activate(IInspectableNode startNode)
    {
        cursorVisual.gameObject.SetActive(true);
        current = startNode;
        current?.SetHighlighted(true);
        if (current != null) cursorVisual.position = current.HighlightTarget.position;
    }

    public void Deactivate()
    {
        current?.SetHighlighted(false);
        current = null;
        cursorVisual.gameObject.SetActive(false);
    }
}