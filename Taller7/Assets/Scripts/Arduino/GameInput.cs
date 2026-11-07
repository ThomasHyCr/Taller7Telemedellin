using UnityEngine;

/// <summary>
/// Entrada unificada: funciona con el teclado y con los pulsadores del circuito a la vez.
/// Úsala en lugar de Input.GetKey / Input.GetKeyDown para W, A, S, D y J.
/// </summary>
public static class GameInput
{
    // El orden coincide con el del circuito: W, A, S, D, J = pines D46..D50
    static int ButtonIndex(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W: return 0;
            case KeyCode.A: return 1;
            case KeyCode.S: return 2;
            case KeyCode.D: return 3;
            case KeyCode.J: return 4;
            default: return -1;
        }
    }

    public static bool GetKeyDown(KeyCode key)
    {
        if (Input.GetKeyDown(key)) return true;

        int index = ButtonIndex(key);
        return index >= 0
            && ArduinoLink.Instance != null
            && ArduinoLink.Instance.GetButtonDown(index);
    }

    public static bool GetKey(KeyCode key)
    {
        if (Input.GetKey(key)) return true;

        int index = ButtonIndex(key);
        return index >= 0
            && ArduinoLink.Instance != null
            && ArduinoLink.Instance.GetButton(index);
    }
}