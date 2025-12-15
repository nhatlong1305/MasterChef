using System;
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public event EventHandler<TutorialStepSO> OnStepChanged;
    public event Action OnTutorialCompleted;

    [Header("Tutorial Steps")]
    public TutorialStepSO[] steps;

    [Header("Highlight Targets")]
    public TutorialHighlightTarget[] highlightTargets;

    [SerializeField] private GameObject tutorialPopup;

    private int currentStepIndex = -1;
    private TutorialStepSO currentStep;
    private BaseCounter cachedTargetCounter;

    private Player player;
    private bool isTutorialRunning = false;
    public bool IsTutorialRunning => isTutorialRunning;

    private const string SKIP_KEY = "SkipTutorial";

    private void Awake()
    {
        Instance = this;
        PlateKitchenObject.OnAnyIngredientAdded += OnPlateIngredientAdded;
    }

    private void Start()
    {
        player = Player.Instance;
        RegisterGameplayEvents();
        StartCoroutine(DelayStart());
    }

    private IEnumerator DelayStart()
    {
        yield return null;

        // ⭐ 1) Check restart tutorial skip BEFORE disabling UI
        if (PlayerPrefs.GetInt(SKIP_KEY, 0) == 1)
        {
            PlayerPrefs.SetInt(SKIP_KEY, 0);
            PlayerPrefs.Save();

            // ẩn popup ngay lập tức
            tutorialPopup.SetActive(false);

            // bật lại UI gameplay (vì restart không dùng tutorial)
            EnableGameplayUI();

            FinishTutorial();  // countdown + gameplay start
            yield break;
        }

        // ⭐ 2) Nếu KHÔNG skip → chạy tutorial như bình thường
        DisableGameplayUI();
        tutorialPopup.SetActive(true);
    }


    private void DisableGameplayUI()
    {
        GameObject.Find("DeliveryManagerUI")?.SetActive(false);
        GameObject.Find("GameStartCountdownUI")?.SetActive(false);
        GameObject.Find("GamePlayingBlockUI")?.SetActive(false);
    }

    private void EnableGameplayUI()
    {
        GameObject.Find("DeliveryManagerUI")?.SetActive(true);
        GameObject.Find("GameStartCountdownUI")?.SetActive(true);
        GameObject.Find("GamePlayingBlockUI")?.SetActive(true);
    }

    // ====================== START / SKIP / END ======================

    public void StartTutorial()
    {
        isTutorialRunning = true;
        currentStepIndex = 0;
        LoadCurrentStep();
    }

    public void SkipTutorial()
    {
        FinishTutorial();
    }

    private void FinishTutorial()
    {
        isTutorialRunning = false;

        foreach (var t in highlightTargets)
            t.target?.DisableOutline();

        currentStep = null;

        EnableGameplayUI();
        OnTutorialCompleted?.Invoke();

        // Bắt đầu countdown
        KitchenGameManager.Instance.StartCountdownAfterTutorial();
    }


    // ====================== STEP HANDLING ======================

    private void LoadCurrentStep()
    {
        player.GameInput.OnInteractAction -= OnInteractCorrectCounter;
        player.GameInput.OnInteractAction -= OnInteractDeliveryCounter;

        if (currentStepIndex >= steps.Length)
        {
            FinishTutorial();
            return;
        }

        currentStep = steps[currentStepIndex];

        // Find target counter
        GameObject obj = null;
        if (!string.IsNullOrEmpty(currentStep.targetCounterName))
            obj = GameObject.Find(currentStep.targetCounterName);

        cachedTargetCounter = obj ? obj.GetComponent<BaseCounter>() : null;

        OnStepChanged?.Invoke(this, currentStep);
        ApplyHighlight(currentStep);

        if (currentStep.conditionType == TutorialConditionType.InteractAtCounter)
        {
            if (player.GetSelectedCounter() == cachedTargetCounter)
                player.GameInput.OnInteractAction += OnInteractCorrectCounter;

            if (currentStep.targetCounterName == "DeliveryCounter")
                player.GameInput.OnInteractAction += OnInteractDeliveryCounter;
        }
    }

    private void NextStep()
    {
        currentStepIndex++;
        LoadCurrentStep();
    }

    // ====================== HIGHLIGHT ======================

    private void ApplyHighlight(TutorialStepSO step)
    {
        foreach (var t in highlightTargets)
            t.target?.DisableOutline();

        foreach (var t in highlightTargets)
        {
            if (t.step == step)
            {
                t.target.EnableOutline();
                break;
            }
        }
    }

    // ====================== UPDATE LOOP ======================

    private void Update()
    {
        if (!isTutorialRunning || currentStep == null) return;

        switch (currentStep.conditionType)
        {
            case TutorialConditionType.MoveWithKeyboard:
                if (player.HasMoved)
                    NextStep();
                break;

            case TutorialConditionType.MoveToCounter:
                CheckMoveToCounter();
                break;
        }
    }

    private void CheckMoveToCounter()
    {
        if (!cachedTargetCounter) return;

        float dist = Vector3.Distance(player.transform.position, cachedTargetCounter.transform.position);

        if (dist < 2f)
            NextStep();
    }

    // ====================== GAMEPLAY EVENTS ======================

    private void RegisterGameplayEvents()
    {
        player.OnPickedSomething += Player_OnPickedSomething;
        player.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        BaseCounter.OnAnyObjectPlaceHere += OnAnyObjectPlaced;

        foreach (var cut in FindObjectsByType<CuttingCounter>(FindObjectsSortMode.None))
            cut.OnCutComplete += OnCutComplete;

        foreach (var plate in FindObjectsByType<PlateKitchenObject>(FindObjectsSortMode.None))
            plate.OnIngredientAdded += OnPlateIngredientAdded;

        foreach (var stove in FindObjectsByType<StoveCounter>(FindObjectsSortMode.None))
        {
            stove.OnStateChanged += Stove_OnStateChanged;
            stove.OnCooked += Stove_OnCooked;
        }
    }

    private void Player_OnPickedSomething(object sender, EventArgs e)
    {
        if (!isTutorialRunning || currentStep == null) return;

        if (currentStep.conditionType == TutorialConditionType.TakeIngredient)
        {
            KitchenObject obj = player.GetKitchenObject();
            if (obj && obj.GetKitchenObjectSO() == currentStep.targetIngredient)
                NextStep();
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (!isTutorialRunning || currentStep == null) return;

        if (currentStep.conditionType != TutorialConditionType.InteractAtCounter) return;

        player.GameInput.OnInteractAction -= OnInteractCorrectCounter;
        player.GameInput.OnInteractAction -= OnInteractDeliveryCounter;

        if (e.selectCounter == cachedTargetCounter)
            player.GameInput.OnInteractAction += OnInteractCorrectCounter;

        if (currentStep.targetCounterName == "DeliveryCounter")
            player.GameInput.OnInteractAction += OnInteractDeliveryCounter;
    }

    private void OnInteractCorrectCounter(object sender, EventArgs e)
    {
        player.GameInput.OnInteractAction -= OnInteractCorrectCounter;
        player.GameInput.OnInteractAction -= OnInteractDeliveryCounter;
        NextStep();
    }

    private void OnInteractDeliveryCounter(object sender, EventArgs e)
    {
        player.GameInput.OnInteractAction -= OnInteractCorrectCounter;
        player.GameInput.OnInteractAction -= OnInteractDeliveryCounter;
        NextStep();
    }

    private void OnAnyObjectPlaced(object sender, EventArgs e)
    {
        if (!isTutorialRunning || currentStep == null) return;
        if (currentStep.conditionType != TutorialConditionType.PlaceIngredient) return;

        if (sender as BaseCounter != cachedTargetCounter) return;

        if (!currentStep.requireSpecificIngredient ||
            cachedTargetCounter.HasIngredient(currentStep.targetIngredient))
        {
            NextStep();
        }
    }

    private void OnPlateIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        if (!isTutorialRunning || currentStep == null) return;
        if (currentStep.conditionType != TutorialConditionType.PlaceIngredient) return;

        if (!currentStep.requireSpecificIngredient ||
            e.kitchenObjectSO == currentStep.targetIngredient)
        {
            NextStep();
        }
    }

    private void OnCutComplete(object sender, EventArgs e)
    {
        if (!isTutorialRunning || currentStep == null) return;
        if (currentStep.conditionType != TutorialConditionType.CutIngredient) return;

        if (currentStep.requireSpecificIngredient)
        {
            KitchenObject obj = (sender as BaseCounter)?.GetKitchenObject();
            if (obj && obj.GetKitchenObjectSO() == currentStep.targetIngredient)
                NextStep();
        }
        else NextStep();
    }

    private void Stove_OnCooked(object sender, EventArgs e)
    {
        if (!isTutorialRunning || currentStep == null) return;
        if (currentStep.conditionType != TutorialConditionType.CookIngredient) return;

        if ((BaseCounter)sender == cachedTargetCounter)
        {
            KitchenObject obj = cachedTargetCounter.GetKitchenObject();
            if (obj && obj.GetKitchenObjectSO() == currentStep.targetIngredient)
                NextStep();
        }
    }

    private void Stove_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        if (!isTutorialRunning || currentStep == null) return;

        if (currentStep.conditionType == TutorialConditionType.TakeCooked &&
            e.state == StoveCounter.State.Fried)
        {
            NextStep();
        }
    }
}
