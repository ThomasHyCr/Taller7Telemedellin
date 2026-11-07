using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Ambiente (ruido de ciudad)")]
    [Tooltip("Con un solo clip se repite en bucle. Con varios, suenan en orden aleatorio sin repetir el mismo dos veces seguidas.")]
    [SerializeField] private AudioClip[] ambientClips;
    [SerializeField, Range(0f, 1f)] private float ambientVolume = 0.5f;

    [Header("Efectos")]
    [Tooltip("Al interactuar con un objeto o pasar un diálogo.")]
    [SerializeField] private AudioClip interactClip;
    [Tooltip("Al cambiar de casilla.")]
    [SerializeField] private AudioClip roomChangeClip;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.8f;
    [Tooltip("Segundos mínimos entre dos sonidos de cambio de casilla.")]
    [SerializeField] private float roomChangeCooldown = 0.25f;

    private AudioSource ambientSource;
    private AudioSource sfxSource;
    private bool ambientPaused;
    private int lastAmbientIndex = -1;
    private float lastRoomSoundTime = float.NegativeInfinity;

    void Awake()
    {
        Instance = this;

        ambientSource = CreateSource(loop: ambientClips != null && ambientClips.Length == 1);
        sfxSource = CreateSource(loop: false);
    }

    void Start()
    {
        GameStateManager.Instance.OnStateChanged += HandleStateChanged;
        DungeonManager.Instance.OnCurrentRoomChanged += HandleRoomChanged;
        ApplyAmbientPause(); // por si el juego arranca en Title
    }

    void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= HandleStateChanged;
        if (DungeonManager.Instance != null)
            DungeonManager.Instance.OnCurrentRoomChanged -= HandleRoomChanged;
    }

    void Update()
    {
        ambientSource.volume = ambientVolume;

        if (ambientPaused || ambientClips == null || ambientClips.Length == 0) return;
        if (!ambientSource.isPlaying) PlayNextAmbient();
    }


    // ---------- Ambiente ----------

    void PlayNextAmbient()
    {
        int index = 0;

        if (ambientClips.Length > 1)
        {
            do { index = Random.Range(0, ambientClips.Length); }
            while (index == lastAmbientIndex);
        }

        lastAmbientIndex = index;
        ambientSource.clip = ambientClips[index];
        ambientSource.Play();
    }

    void HandleStateChanged(GameState previous, GameState current)
    {
        ApplyAmbientPause();
    }

    void ApplyAmbientPause()
    {
        // Se lee el estado real y no el del evento: si un cambio de estado provoca otro enseguida
        // (por ejemplo, al cerrar el panel de inicio y arrancar el video), el evento llegaría desactualizado.
        var state = GameStateManager.Instance.CurrentState;
        ambientPaused = state == GameState.Title || state == GameState.PlayingCutscene;

        if (ambientPaused) ambientSource.Pause();
        else ambientSource.UnPause();
    }

    // ---------- Efectos ----------

    /// <summary>Sonido corto de interacción (objeto o diálogo).</summary>
    public void PlayInteractSound()
    {
        PlaySfx(interactClip);
    }

    void HandleRoomChanged(RoomNode room)
    {
        // La sala inicial no tiene sala anterior: sin sonido al arrancar la escena
        if (DungeonManager.Instance.PreviousRoom == null) return;

        if (Time.unscaledTime - lastRoomSoundTime < roomChangeCooldown) return;
        lastRoomSoundTime = Time.unscaledTime;

        PlaySfx(roomChangeClip);
    }

    void PlaySfx(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // ---------- Utilidades ----------

    AudioSource CreateSource(bool loop)
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        source.spatialBlend = 0f; // sonido 2D
        return source;
    }
}