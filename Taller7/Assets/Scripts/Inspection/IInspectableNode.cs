using UnityEngine;

public interface IInspectableNode
{
    IInspectableNode Up { get; }
    IInspectableNode Down { get; }
    IInspectableNode Left { get; }
    IInspectableNode Right { get; }
    Transform HighlightTarget { get; }

    void SetHighlighted(bool value);
    void Interact();
}