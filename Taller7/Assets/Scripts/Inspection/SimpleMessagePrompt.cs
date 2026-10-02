using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleMessagePrompt : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;

    private Action onDismiss;

    void Awake()
    {
        Debug.Log($"[DEBUG] Awake de SimpleMessagePrompt en GameObject '{gameObject.name}' (ID={gameObject.GetInstanceID()}). panel field apunta a '{panel.name}' (ID={panel.GetInstanceID()})");
        panel.SetActive(false);
    }

    void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.J))
        {
            panel.SetActive(false);
            var callback = onDismiss;
            onDismiss = null;
            callback?.Invoke();
        }
    }

    public void ShowMessage(string message, Action onDismissed)
    {
        Debug.Log($"[DEBUG] ShowMessage en script de GameObject ID={gameObject.GetInstanceID()}. panel ID={panel.GetInstanceID()}, activeSelf antes={panel.activeSelf}");
        messageText.text = message;
        onDismiss = onDismissed;
        panel.SetActive(true);
        Debug.Log($"[DEBUG] Inmediatamente después de SetActive(true): activeSelf={panel.activeSelf}");
    }
}