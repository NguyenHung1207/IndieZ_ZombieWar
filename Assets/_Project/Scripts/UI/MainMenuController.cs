using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

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

    private void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(PlayGame);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);
    }
}
