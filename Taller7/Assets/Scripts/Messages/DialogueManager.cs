using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject panel;        // el panel visual del mensaje
    [SerializeField] private TMP_Text messageText;

    [Header("Personaje")]
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite[] characterSprites;     // tus 3 sprites
    [SerializeField] private bool changeEveryLine = true;   // false = solo cambia al abrir un mensaje nuevo

    private int lastSpriteIndex = -1;

    private class Entry
    {
        public string[] Lines;
        public Action OnFinished;
    }

    private readonly Queue<Entry> pending = new();
    private Entry current;
    private int lineIndex;
    private GameState stateBefore;
    private int shownFrame = -1;

    public bool IsActive { get; private set; }

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    void LateUpdate()
    {
        if (!IsActive) return;
        if (Time.frameCount == shownFrame) return; // ignora la misma pulsación que abrió el mensaje
        if (!GameInput.GetKeyDown(KeyCode.J)) return;
        
        AudioManager.Instance.PlayInteractSound();
        lineIndex++;
        if (lineIndex < current.Lines.Length) ShowCurrentLine();
        else FinishCurrent();
    }

    /// <summary>
    /// Muestra un mensaje de una o varias páginas. Se avanza con J.
    /// onFinished se llama al cerrar, con el estado del juego ya restaurado.
    /// </summary>
    public void Show(IList<string> lines, Action onFinished = null)
    {
        if (lines == null || lines.Count == 0)
        {
            onFinished?.Invoke();
            return;
        }

        var entry = new Entry { Lines = new string[lines.Count], OnFinished = onFinished };
        lines.CopyTo(entry.Lines, 0);

        if (IsActive) pending.Enqueue(entry);
        else Begin(entry);
    }

    void Begin(Entry entry)
    {
        current = entry;
        lineIndex = 0;

        if (!IsActive)
        {
            stateBefore = GameStateManager.Instance.CurrentState;
            GameStateManager.Instance.SetState(GameState.ShowingMessage);
            IsActive = true;
            panel.SetActive(true);
        }

        shownFrame = Time.frameCount;
        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        messageText.text = current.Lines[lineIndex];

        if (changeEveryLine || lineIndex == 0)
        PickRandomCharacter();
    }

    void PickRandomCharacter()
    {
        if (characterImage == null || characterSprites == null || characterSprites.Length == 0) return;

        int index = 0;

        if (characterSprites.Length > 1)
        {
        do { index = UnityEngine.Random.Range(0, characterSprites.Length); }
        while (index == lastSpriteIndex);
        }

        lastSpriteIndex = index;
        characterImage.sprite = characterSprites[index];
    }

    void FinishCurrent()
    {
        var finished = current;
        current = null;

        if (pending.Count > 0)
        {
            finished.OnFinished?.Invoke();
            Begin(pending.Dequeue());
            return;
        }

        IsActive = false;
        panel.SetActive(false);
        GameStateManager.Instance.SetState(stateBefore);

        finished.OnFinished?.Invoke();
    }
}