using System.Collections.Generic;
using UnityEngine;

public class InspectionCursorController : MonoBehaviour
{
    [SerializeField] private RectTransform cursorVisual;
    [SerializeField] private float moveSpeed = 600f; // píxeles por segundo
    [SerializeField] private Camera uiCamera; // deja en None si el Canvas es Screen Space - Overlay

    private List<IInspectable> currentNodes = new();
    private IInspectable hovered;
    private RectTransform boundsRect;
    private Transform originalParent;

    void Awake()
    {
        originalParent = cursorVisual.parent;
    }

    void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Inspecting) return;

        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) input.y += 1;
        if (Input.GetKey(KeyCode.S)) input.y -= 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;
        if (Input.GetKey(KeyCode.D)) input.x += 1;

        if (input != Vector2.zero)
            MoveCursor(input.normalized * moveSpeed * Time.deltaTime);

        UpdateHover();

        if (Input.GetKeyDown(KeyCode.J))
            hovered?.Interact();
    }

    void MoveCursor(Vector2 delta)
    {
        Vector2 pos = cursorVisual.anchoredPosition + delta;

        if (boundsRect != null)
        {
            Rect rect = boundsRect.rect;
            pos.x = Mathf.Clamp(pos.x, rect.xMin, rect.xMax);
            pos.y = Mathf.Clamp(pos.y, rect.yMin, rect.yMax);
        }

        cursorVisual.anchoredPosition = pos;
    }

    void UpdateHover()
    {
        IInspectable newHovered = null;

        foreach (var node in currentNodes)
        {
            if (node == null) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(node.RectTransform, cursorVisual.position, uiCamera))
            {
                newHovered = node;
                break;
            }
        }

        if (newHovered == hovered) return;

        hovered?.SetHighlighted(false);
        hovered = newHovered;
        hovered?.SetHighlighted(true);
    }

    /// <summary>
    /// Activa el cursor dentro de una sala especial.
    /// </summary>
    /// <param name="nodes">Objetos inspeccionables + botón volver de esa sala.</param>
    /// <param name="bounds">RectTransform del panel: limita el movimiento del cursor a esa área.</param>
    /// <param name="startAnchoredPosition">Posición local (dentro de bounds) donde aparece el cursor.</param>
    public void Activate(List<IInspectable> nodes, RectTransform bounds, Vector2 startAnchoredPosition)
    {
        currentNodes = nodes;
        boundsRect = bounds;
        hovered = null;

        cursorVisual.SetParent(bounds, worldPositionStays: false);
        cursorVisual.anchoredPosition = startAnchoredPosition;
        cursorVisual.gameObject.SetActive(true);

        UpdateHover();
    }

    public void Deactivate()
    {
        hovered?.SetHighlighted(false);
        hovered = null;
        currentNodes.Clear();
        boundsRect = null;

        cursorVisual.gameObject.SetActive(false);
        cursorVisual.SetParent(originalParent, worldPositionStays: false);
    }
}