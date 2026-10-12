using UnityEngine;

public class SceneIntroMessage : MonoBehaviour
{
    [SerializeField, TextArea(2, 4)] private string[] introLines =
    {
        "Usa W, A, S y D para moverte entre los lugares del mapa.",
        "Pulsa J para avanzar los mensajes e interactuar con los objetos.",
        "Los lugares que brillan en amarillo tienen algo que inspeccionar."
    };

    void Start()
    {
        DialogueManager.Instance.Show(introLines);
    }
}