using System;
using System.Collections;
using UnityEngine;

public class CutscenePlayer : MonoBehaviour
{
    public static CutscenePlayer Instance;
    void Awake() => Instance = this;

    // TODO: reemplazar por VideoPlayer real (clips de 30-45s) cuando se implemente esa parte.
    public void PlayInspectionCutscene(Action onFinished)
    {
        StartCoroutine(FakeCutsceneRoutine(onFinished));
    }

    IEnumerator FakeCutsceneRoutine(Action onFinished)
    {
        GameStateManager.Instance.SetState(GameState.PlayingCutscene);
        yield return new WaitForSeconds(2f); // placeholder de duración
        onFinished?.Invoke();
    }
}