using UnityEngine;
using UnityEngine.UI;

public sealed class GameHUD : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text resultText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Button restartButton;

    private GameSession session;
    private PlayerHealth playerHealth;

    private void Start()
    {
        session = GameSession.Instance;
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (session != null)
            session.StateChanged += HandleStateChanged;
        if (restartButton != null && session != null)
            restartButton.onClick.AddListener(session.RestartLevel);
        HandleStateChanged(session != null ? session.State : GameSessionState.Playing);
    }

    private void Update()
    {
        if (session != null && timerText != null)
        {
            int totalSeconds = Mathf.CeilToInt(session.RemainingTime);
            timerText.text = string.Format("TIME  {0:00}:{1:00}", totalSeconds / 60, totalSeconds % 60);
        }
        if (playerHealth != null && healthText != null)
            healthText.text = string.Format("HP  {0:000}", Mathf.CeilToInt(playerHealth.CurrentHealth));
    }

    private void HandleStateChanged(GameSessionState state)
    {
        bool ended = state != GameSessionState.Playing;
        if (resultPanel != null)
            resultPanel.SetActive(ended);
        if (resultText != null)
            resultText.text = state == GameSessionState.Victory ? "VICTORY" : "GAME OVER";
    }

    private void OnDestroy()
    {
        if (session != null)
            session.StateChanged -= HandleStateChanged;
        if (restartButton != null && session != null)
            restartButton.onClick.RemoveListener(session.RestartLevel);
    }
}
