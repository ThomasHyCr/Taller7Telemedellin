using System;
using UnityEngine;
using UnityEngine.UI;

public class SimpleMessagePrompt : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Text messageText;

    private Action onDismiss;

    void Awake() => panel.SetActive(false);

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
        messageText.text = message;
        onDismiss = onDismissed;
        panel.SetActive(true);
    }
}