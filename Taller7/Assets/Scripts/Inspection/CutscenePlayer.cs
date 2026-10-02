using System;
using UnityEngine;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour
{
    public static CutscenePlayer Instance;

    [SerializeField] private GameObject overlay;      // Panel de pantalla completa, opaco, inicialmente desactivado
    [SerializeField] private VideoPlayer videoPlayer;

    private Action onFinishedCallback;

    void Awake()
    {
        Instance = this;
        overlay.SetActive(false);

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += HandleVideoFinished;
    }

    public void PlayInspectionCutscene(VideoClip clip, Action onFinished)
    {
        if (clip == null)
        {
            Debug.LogWarning("PlayInspectionCutscene llamado sin VideoClip asignado — se salta la cutscene.");
            onFinished?.Invoke();
            return;
        }

        onFinishedCallback = onFinished;
        GameStateManager.Instance.SetState(GameState.PlayingCutscene);

        overlay.SetActive(true);
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    void HandleVideoFinished(VideoPlayer vp)
    {
        overlay.SetActive(false);
        videoPlayer.clip = null;

        var callback = onFinishedCallback;
        onFinishedCallback = null;
        callback?.Invoke();
    }
}