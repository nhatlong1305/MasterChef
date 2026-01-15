using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    // ================= EVENTS =================
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    // ================= REFERENCES =================
    private PlayerInputActions playerInputActions;
    private KitchenGameManager kitchenGameManager;
    private TutorialManager tutorialManager;

    // ================= LIFECYCLE =================
    private void Awake()
    {
        kitchenGameManager = KitchenGameManager.Instance;
        tutorialManager = TutorialManager.Instance;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        // Register input
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        if (playerInputActions == null) return;

        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }

    // ================= INTERACT =================
    private void Interact_performed(InputAction.CallbackContext context)
    {
        if (IsInputBlocked()) return;
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(InputAction.CallbackContext context)
    {
        if (IsInputBlocked()) return;
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    // ================= PAUSE =================
    private void Pause_performed(InputAction.CallbackContext context)
    {
        // Pause vẫn cho bấm kể cả khi đang chơi
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    // ================= MOVEMENT =================
    public Vector2 GetMovementVectorNormalized()
    {
        // ⛔ GameOver → đứng yên
        if (kitchenGameManager.IsGameOver())
            return Vector2.zero;

        // ⏸ Pause → đứng yên
        if (GamePauseManager.Instance != null &&
            GamePauseManager.Instance.IsPaused())
            return Vector2.zero;

        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        // Tutorial đang chạy → vẫn cho di chuyển
        if (tutorialManager != null && tutorialManager.IsTutorialRunning)
            return inputVector.normalized;

        // Game đang chơi → cho di chuyển
        if (kitchenGameManager.IsGamePlaying())
            return inputVector.normalized;

        // Các state khác → không di chuyển
        return Vector2.zero;
    }

    // ================= HELPERS =================
    private bool IsInputBlocked()
    {
        // ⛔ GameOver
        if (kitchenGameManager.IsGameOver())
            return true;

        // ⛔ Countdown
        if (kitchenGameManager.IsCountdownToStartActive())
            return true;

        // ⛔ Pause
        if (GamePauseManager.Instance != null &&
            GamePauseManager.Instance.IsPaused())
            return true;

        return false;
    }
}
