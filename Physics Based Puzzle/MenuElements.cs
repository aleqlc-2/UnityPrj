using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuElements : MonoBehaviour
{
    public string sceneToLoad;

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
