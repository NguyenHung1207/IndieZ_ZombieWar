using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class PauseController : MonoBehaviour
{
    public static bool IsPaused { get; private set; }
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    private bool paused;

    private void Start()
    {
        if (pauseButton != null) pauseButton.onClick.AddListener(Pause);
        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    public void Pause()
    {
        if (paused || (GameSession.Instance != null && !GameSession.Instance.IsPlaying)) return;
        paused = true; IsPaused = true; Time.timeScale = 0f;
        foreach (PlayerWeaponController w in FindObjectsByType<PlayerWeaponController>(FindObjectsSortMode.None)) w.StopInput();
        foreach (VirtualJoystick j in FindObjectsByType<VirtualJoystick>(FindObjectsSortMode.None)) j.ResetInput();
        if (pausePanel != null) pausePanel.SetActive(true);
    }
    public void Resume()
    {
        paused = false; IsPaused = false; Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    public void ReturnToMainMenu() { IsPaused = false; Time.timeScale = 1f; SceneManager.LoadScene("MainMenu"); }
    private void OnDestroy() { IsPaused = false; Time.timeScale = 1f; }
}
