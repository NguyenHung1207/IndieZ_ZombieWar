using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Gameplay_Level01");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit requested from Main Menu (ignored in the Editor).");
#else
        Application.Quit();
#endif
    }
}
