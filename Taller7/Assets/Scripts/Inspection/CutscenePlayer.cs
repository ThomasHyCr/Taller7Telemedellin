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

    /// <summary>
    /// Reproduce un video a pantalla completa. Mientras dura, el estado del juego es PlayingCutscene.
    /// Al terminar, el estado NO se restaura: quien llama decide cuál sigue en onFinished.
    /// </summary>
    public void Play(VideoClip clip, Action onFinished)
    {
        if (clip == null)
        {
            Debug.LogWarning("CutscenePlayer.Play llamado sin VideoClip asignado — se salta la animación.");
            onFinished?.Invoke();
            return;
        }

        onFinishedCallback = onFinished;
        GameStateManager.Instance.SetState(GameState.PlayingCutscene);

        overlay.SetActive(true);
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    // Se mantiene el nombre anterior para no tener que tocar InspectableObject
    public void PlayInspectionCutscene(VideoClip clip, Action onFinished) => Play(clip, onFinished);

    void HandleVideoFinished(VideoPlayer vp)
    {
        overlay.SetActive(false);
        videoPlayer.clip = null;

        var callback = onFinishedCallback;
        onFinishedCallback = null;
        callback?.Invoke();
    }
}