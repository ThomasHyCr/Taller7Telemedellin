using UnityEngine;

public class InspectableObject : MonoBehaviour, IInspectableNode
{
    [Header("Navegación (WASD) — arrastra otro InspectableObject o el ReturnButtonNode")]
    [SerializeField] private MonoBehaviour up, down, left, right;

    [Header("Configuración")]
    [SerializeField] private bool isCorrectObject;
    [SerializeField] private GameObject highlightVisual; // outline/glow, desactivado por defecto

    public IInspectableNode Up => up as IInspectableNode;
    public IInspectableNode Down => down as IInspectableNode;
    public IInspectableNode Left => left as IInspectableNode;
    public IInspectableNode Right => right as IInspectableNode;
    public Transform HighlightTarget => transform;

    public void SetHighlighted(bool value)
    {
        if (highlightVisual != null) highlightVisual.SetActive(value);
    }

    public void Interact()
    {
        if (!isCorrectObject)
        {
            // TODO: cuando exista el sistema de diálogos, mostrar algo tipo "no parece nada interesante"
            Debug.Log("Objeto incorrecto (sin efecto por ahora).");
            return;
        }

        CutscenePlayer.Instance.PlayInspectionCutscene(OnCutsceneFinished);
    }

    void OnCutsceneFinished()
    {
        var room = DungeonManager.Instance.CurrentRoom;
        room.SpecialSolved = true;

        SpecialRoomRegistry.Instance.DeactivateRoom(room.GridPosition);
        GameStateManager.Instance.SetState(GameState.Exploring);
    }
}