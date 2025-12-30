using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private bool isPaused;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private GameObject deliveryManagerUI;
    [SerializeField] private GameObject gameStartCountdownUI;
    [SerializeField] private GameObject gamePlayingBlockUI;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void OnPlayButton()
    {
        SoundManager.Instance.PlayUIClickSound();
        Loader.Load(Loader.Scene.Kitchen);
    }

    public void OnQuitButton()
    {
        SoundManager.Instance.PlayUIClickSound();
        Application.Quit();
    }



    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }


    public void OnGameOver()
    {
        Debug.Log("Game Over");

        Time.timeScale = 0f;

       
        if (tutorialUI != null) tutorialUI.SetActive(false);
        if (deliveryManagerUI != null) deliveryManagerUI.SetActive(false);
        if (gameStartCountdownUI != null) gameStartCountdownUI.SetActive(false);
        if (gamePlayingBlockUI != null) gamePlayingBlockUI.SetActive(false);

        
        if (gameOverUI != null) gameOverUI.SetActive(true);
    }

    public void OnRestartButton()
    {
        SoundManager.Instance.PlayUIClickSound();
        Time.timeScale = 1f;
        Loader.Load(Loader.Scene.Kitchen);
    }

    public void OnBackToMenuButton()
    {
        SoundManager.Instance.PlayUIClickSound();
        Time.timeScale = 1f;
        Loader.Load(Loader.Scene.GameMenuScenes);
    }
}
