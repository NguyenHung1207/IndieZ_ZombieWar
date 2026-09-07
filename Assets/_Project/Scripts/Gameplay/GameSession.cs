using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameSessionState
{
    Playing,
    Victory,
    GameOver
}

public sealed class GameSession : MonoBehaviour
{
    [SerializeField, Min(1f)] private float durationSeconds = 180f;
#if UNITY_EDITOR
    [SerializeField, Min(0f)] private float editorTestDurationOverride;
#endif

    private PlayerHealth playerHealth;
    private float elapsedTime;

    public static GameSession Instance { get; private set; }
    public event Action<GameSessionState> StateChanged;
    public GameSessionState State { get; private set; }
    public float DurationSeconds => durationSeconds;
    public float ElapsedTime => elapsedTime;
    public float Progress01 => Mathf.Clamp01(elapsedTime / ActiveDuration);
    public float RemainingTime => Mathf.Max(0f, ActiveDuration - elapsedTime);
    public bool IsPlaying => State == GameSessionState.Playing;

    private float ActiveDuration
    {
        get
        {
#if UNITY_EDITOR
            if (editorTestDurationOverride > 0f)
                return editorTestDurationOverride;
#endif
            return durationSeconds;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        State = GameSessionState.Playing;
    }

    private void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.Died += HandlePlayerDied;
        StateChanged?.Invoke(State);
    }

    private void Update()
    {
        if (!IsPlaying)
            return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= ActiveDuration)
            End(GameSessionState.Victory);
    }

    private void HandlePlayerDied()
    {
        End(GameSessionState.GameOver);
    }

    private void End(GameSessionState endState)
    {
        if (!IsPlaying)
            return;

        State = endState;
        if (endState == GameSessionState.Victory)
            elapsedTime = ActiveDuration;
        StateChanged?.Invoke(State);

        foreach (ZombieAI zombie in FindObjectsByType<ZombieAI>(FindObjectsSortMode.None))
            zombie.StopForGameEnd();
        foreach (ZombieSpawnDirector spawner in FindObjectsByType<ZombieSpawnDirector>(FindObjectsSortMode.None))
            spawner.enabled = false;
        foreach (PlayerMovement movement in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
            movement.enabled = false;
        foreach (PlayerWeaponController weapons in FindObjectsByType<PlayerWeaponController>(FindObjectsSortMode.None))
        {
            weapons.StopInput();
            weapons.enabled = false;
        }
        foreach (PlayerGrenadeController grenades in FindObjectsByType<PlayerGrenadeController>(FindObjectsSortMode.None))
            grenades.enabled = false;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.Died -= HandlePlayerDied;
        if (Instance == this)
            Instance = null;
    }
}
