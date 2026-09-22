using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainViewDisplay : MonoBehaviour
{
    [SerializeField] private Image imageA;
    [SerializeField] private Image imageB;
    [SerializeField] private float fadeDuration = 0.35f;

    private Image current, next;
    private Coroutine fadeRoutine;

    void Awake()
    {
        current = imageA;
        next = imageB;
        next.color = new Color(1, 1, 1, 0);
    }

    void Start()
    {
        DungeonManager.Instance.OnCurrentRoomChanged += HandleRoomChanged;

        if (DungeonManager.Instance.CurrentRoom != null)
            current.sprite = DungeonManager.Instance.CurrentRoom.RoomImage;
    }

    void OnDestroy()
    {
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.OnCurrentRoomChanged -= HandleRoomChanged;
    }

    void HandleRoomChanged(RoomNode room)
    {
        if (room.RoomImage == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(CrossFade(room.RoomImage));
    }

    IEnumerator CrossFade(Sprite newSprite)
    {
        next.sprite = newSprite;
        next.color = new Color(1, 1, 1, 0);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = t / fadeDuration;
            next.color = new Color(1, 1, 1, a);
            current.color = new Color(1, 1, 1, 1 - a);
            yield return null;
        }

        next.color = Color.white;
        current.color = Color.white;

        (current, next) = (next, current);
    }
}