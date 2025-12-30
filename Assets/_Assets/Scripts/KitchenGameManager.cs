using System;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnGamePlayingStarted;
    public event EventHandler OnStateChanged;
    public event EventHandler OnGameOver;

    public enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;

    [Header("Countdown")]
    [SerializeField] private float countdownToStartTimerMax = 3f;
    private float countdownToStartTimer;

    [Header("Game Playing")]
    [SerializeField] private float gamePlayingTimerMax = 120f;
    private float gamePlayingTimer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        state = State.WaitingToStart;
    }

    private void Update()
    {
        switch (state)
        {
            case State.CountdownToStart:
                HandleCountdown();
                break;

            case State.GamePlaying:
                HandleGamePlaying();
                break;

            case State.GameOver:
              
                break;
        }
    }


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


    private void SetState(State newState)
    {
        state = newState;

        switch (state)
        {
            case State.CountdownToStart:
                countdownToStartTimer = countdownToStartTimerMax;
                break;

            case State.GamePlaying:
                gamePlayingTimer = gamePlayingTimerMax;
                break;

            case State.GameOver:
                break;
        }

        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    
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
