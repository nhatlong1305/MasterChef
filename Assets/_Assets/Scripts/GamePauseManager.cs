using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pauseUI;

    private bool isPaused;

    // ================= LIFECYCLE =================
    private void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (pauseUI != null)
            pauseUI.SetActive(false);
        
    }

    private void Start()
    {
        GameInput gameInput = FindAnyObjectByType<GameInput>();
        if (gameInput != null)
        {
            gameInput.OnPauseAction += OnPausePressed;
        }
      
    }

    private void OnDestroy()
    {
        GameInput gameInput = FindAnyObjectByType<GameInput>();
        if (gameInput != null)
        {
            gameInput.OnPauseAction -= OnPausePressed;
        }
    }

    // ================= PAUSE LOGIC =================
    private void OnPausePressed(object sender, EventArgs e)
    {
        if (KitchenGameManager.Instance != null &&
            KitchenGameManager.Instance.IsGameOver())
            return;

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    // ================= PUBLIC API (BUTTON CALL) =================
    public void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (pauseUI != null)
            pauseUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pauseUI != null)
            pauseUI.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("GameMenuScenes");
    }

    // ================= GETTER =================   
    public bool IsPaused() => isPaused;
}
