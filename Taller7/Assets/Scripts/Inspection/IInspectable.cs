using UnityEngine;

public interface IInspectable
{
    RectTransform RectTransform { get; }

    void SetHighlighted(bool value);
    void Interact();
}