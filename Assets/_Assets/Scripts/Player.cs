using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }
    public static Player instanceField;

    public static Player GetInstanceField()
    {
        return instanceField;
    }

    public static void SetInstanceField(Player instanceField)
    {
        Player.instanceField = instanceField;
    }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectCounter;
    }

    public bool HasMoved { get; private set; }
    public bool HasInteracted { get; private set; }
    public bool HasPickedIngredient { get; private set; }

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private GameInput gameInput;
    public GameInput GameInput => gameInput;

    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private KitchenObject kitchenObject;
    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;

    private void Awake()
    {
        
        Instance = this;
        instanceField = this;

   
        lastInteractDir = transform.forward;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteracAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteracAlternateAction;
    }


    private void OnDestroy()
    {
        
        if (gameInput != null)
        {
            gameInput.OnInteractAction -= GameInput_OnInteracAction;
            gameInput.OnInteractAlternateAction -= GameInput_OnInteracAlternateAction;
        }
    }

    private void Update()
    {
        HandleInteractions();
        HandleMovement();
    }

    private void GameInput_OnInteracAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()
            && !TutorialManager.Instance.IsTutorialRunning)
            return;

        HasInteracted = true;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteracAlternateAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()
            && !TutorialManager.Instance.IsTutorialRunning)
            return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }


    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            HasMoved = true;
        }

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;

        bool canMove = !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * playerHeight,
            playerRadius,
            moveDir,
            moveDistance
        );

        if (!canMove && moveDir != Vector3.zero)
        {
         
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            bool canMoveX = moveDir.x != 0 && !Physics.CapsuleCast(
                transform.position,
                transform.position + Vector3.up * playerHeight,
                playerRadius,
                moveDirX,
                moveDistance
            );

            if (canMoveX)
            {
                moveDir = moveDirX;
                canMove = true;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                bool canMoveZ = moveDir.z != 0 && !Physics.CapsuleCast(
                    transform.position,
                    transform.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirZ,
                    moveDistance
                );

                if (canMoveZ)
                {
                    moveDir = moveDirZ;
                    canMove = true;
                }
                else
                {
                    canMove = false;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        if (moveDir != Vector3.zero)
        {
            float rotateSpeed = 5f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectCounter = selectedCounter
        });
    }
    public BaseCounter GetSelectedCounter()
    {
        return selectedCounter;
    }


    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            HasPickedIngredient = true;

            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public void ResetTutorialFlags()
    {
        HasMoved = false;
        HasInteracted = false;
        HasPickedIngredient = false;
    }
}
