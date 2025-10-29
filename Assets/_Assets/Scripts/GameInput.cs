using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions(); 
        playerInputActions.Player.Enable();            

        playerInputActions.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        // Nếu bạn muốn dùng Input System thay vì GetKey:
        // Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        Vector2 inputVector = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) inputVector.x = +1;
        if (Input.GetKey(KeyCode.S)) inputVector.x = -1;
        if (Input.GetKey(KeyCode.A)) inputVector.y = +1;
        if (Input.GetKey(KeyCode.D)) inputVector.y = -1;

        return inputVector.normalized;
    }
}
