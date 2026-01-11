using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;

    private PlayerInputActions playerInputActions;
    private KitchenGameManager kitchenGameManager;
    private TutorialManager tutorialManager;

    private void Awake()
    {
        kitchenGameManager = KitchenGameManager.Instance;
        tutorialManager = TutorialManager.Instance;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
    }

    private void OnDestroy()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Interact.performed -= Interact_performed;
            playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
            playerInputActions.Dispose();
        }
    }

    // ================= INTERACT =================

    private void Interact_performed(InputAction.CallbackContext context)
    {
        if (kitchenGameManager.IsGameOver()) return;

       
        if (kitchenGameManager.IsCountdownToStartActive()) return;

       
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(InputAction.CallbackContext context)
    {
        if (kitchenGameManager.IsGameOver()) return;
        if (kitchenGameManager.IsCountdownToStartActive()) return;

        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    // ================= MOVEMENT =================

    public Vector2 GetMovementVectorNormalized()
    {
        
        if (kitchenGameManager.IsGameOver())
            return Vector2.zero;

       
        if (tutorialManager != null && tutorialManager.IsTutorialRunning)
        {
            Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
            return inputVector.normalized;
        }

        
        if (kitchenGameManager.IsGamePlaying())
        {
            Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
            return inputVector.normalized;
        }

       
        return Vector2.zero;
    }
}
