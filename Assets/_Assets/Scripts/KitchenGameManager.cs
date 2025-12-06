using System;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnGamePlayingStarted;
    public event EventHandler OnStateChanged;
    public event EventHandler OnGameOver;

    private enum State
    {
        WaitingToStart,      
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;

    private float countdownToStartTimer = 3f;
    private float countdownToStartTimerMax = 3f;

    private float gamePlayingTimer;
    [SerializeField] private float gamePlayingTimerMax = 5f;

    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
    }


    private void Update()
    {
        switch (state)
        {
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;

                    gamePlayingTimer = gamePlayingTimerMax;

                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    OnGamePlayingStarted?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    OnGameOver?.Invoke(this, EventArgs.Empty);
                }
                break;
        }
    }


    public bool IsCountdownToStartActive() => state == State.CountdownToStart;
    public bool IsGamePlaying() => state == State.GamePlaying;
    public bool IsGameOver() => state == State.GameOver;

    public float GetCountdownToStartTime() => countdownToStartTimer;

    public float GetGamePlayingTimerNormalized()
        => 1 - (gamePlayingTimer / gamePlayingTimerMax);


    public void StartCountdownAfterTutorial()
    {
        state = State.CountdownToStart;
        countdownToStartTimer = countdownToStartTimerMax;

        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
}
