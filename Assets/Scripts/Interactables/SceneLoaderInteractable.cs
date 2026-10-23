using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string sceneName;

    public void Interact()
    {
        SceneManager.LoadScene(sceneName);
    }
}
