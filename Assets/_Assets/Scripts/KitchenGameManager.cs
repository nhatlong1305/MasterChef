using System;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    // ================= EVENTS =================
    public event EventHandler OnGamePlayingStarted;
    public event EventHandler OnStateChanged;
    public event EventHandler OnGameOver;

    // ================= GAME STATE =================
    public enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;

    // ================= COUNTDOWN =================
    [Header("Countdown")]
    [SerializeField] private float countdownToStartTimerMax = 3f;
    private float countdownToStartTimer;

    // ================= GAME PLAYING =================
    [Header("Game Playing")]
    [SerializeField] private float gamePlayingTimerMax = 120f;
    private float gamePlayingTimer;

    // ================= LIFECYCLE =================
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        state = State.WaitingToStart;

        ShowCursor();
    }

    private void Update()
    {
        // ⏸ PAUSE → KHÔNG UPDATE TIMER
        if (GamePauseManager.Instance != null &&
            GamePauseManager.Instance.IsPaused())
        {
            return;
        }

        switch (state)
        {
            case State.CountdownToStart:
                HandleCountdown();
                break;

            case State.GamePlaying:
                HandleGamePlaying();
                break;
        }
    }

    // ================= HANDLERS =================
    private void HandleCountdown()
    {
        countdownToStartTimer -= Time.deltaTime;

        if (countdownToStartTimer <= 0f)
        {
            SetState(State.GamePlaying);
            OnGamePlayingStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HandleGamePlaying()
    {
        gamePlayingTimer -= Time.deltaTime;

        if (gamePlayingTimer <= 0f)
        {
            gamePlayingTimer = 0f;
            SetState(State.GameOver);
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }
    }

    // ================= STATE =================
    private void SetState(State newState)
    {
        state = newState;

        switch (state)
        {
            case State.WaitingToStart:
                HideCursor();
                break;

            case State.CountdownToStart:
                countdownToStartTimer = countdownToStartTimerMax;
                HideCursor();
                break;

            case State.GamePlaying:
                gamePlayingTimer = gamePlayingTimerMax;
                HideCursor();
                break;

            case State.GameOver:
                ShowCursor();
                break;
        }

        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    // ================= CURSOR =================
    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // ================= PUBLIC API =================
    public void StartCountdownAfterTutorial()
    {
        if (state != State.WaitingToStart) return;
        SetState(State.CountdownToStart);
    }

    public bool IsCountdownToStartActive() => state == State.CountdownToStart;
    public bool IsGamePlaying() => state == State.GamePlaying;
    public bool IsGameOver() => state == State.GameOver;

    public float GetCountdownToStartTime()
    {
        return Mathf.CeilToInt(Mathf.Max(countdownToStartTimer, 0f));
    }

    public float GetGamePlayingTimerNormalized()
    {
        if (gamePlayingTimerMax <= 0f) return 0f;
        return 1f - (gamePlayingTimer / gamePlayingTimerMax);
    }
}
