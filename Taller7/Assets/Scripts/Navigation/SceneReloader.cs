using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Recarga la escena actual: con la tecla R en cualquier momento,
/// o llamando a SceneReloader.Reload() desde otro script.
/// </summary>
public class SceneReloader : MonoBehaviour
{
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    void Update()
    {
        if (Input.GetKeyDown(reloadKey))
            Reload();
    }

    public static void Reload()
    {
        var scene = SceneManager.GetActiveScene();

        if (scene.buildIndex < 0)
        {
            Debug.LogError("No se puede recargar la escena: no está en la lista de escenas del build. " +
                           "Ve a File > Build Profiles > Scene List y pulsa 'Add Open Scenes'.");
            return;
        }

        SceneManager.LoadScene(scene.buildIndex);
    }
}