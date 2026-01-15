using System;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    // 🔥 Event cho Tutorial
    public event EventHandler OnCooked;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    // 🔊 AUDIO LOOP STOVE
    [SerializeField] private AudioSource stoveAudioSource;

    private State state;
    private float fryingTimer;
    private float burningTimer;

    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    // =====================================================
    // =================== LIFECYCLE ======================
    // =====================================================

    private void Start()
    {
        state = State.Idle;
        StopStoveSound();

        if (KitchenGameManager.Instance != null)
            KitchenGameManager.Instance.OnGameOver += OnGameOver;
    }

    private void OnDestroy()
    {
        if (KitchenGameManager.Instance != null)
            KitchenGameManager.Instance.OnGameOver -= OnGameOver;
    }

    // =====================================================
    // =================== UPDATE =========================
    // =====================================================

    private void Update()
    {
        // ⏸ PAUSE → DỪNG TOÀN BỘ LOGIC
        if (GamePauseManager.Instance != null &&
            GamePauseManager.Instance.IsPaused())
        {
            StopStoveSound();
            return;
        }

        // 🔥 GAME OVER → TẮT NGAY
        if (KitchenGameManager.Instance != null &&
            KitchenGameManager.Instance.IsGameOver())
        {
            StopStoveSound();
            return;
        }

        if (!HasKitchenObject()) return;

        switch (state)
        {
            case State.Frying:
                fryingTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this,
                    new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized =
                            fryingTimer / fryingRecipeSO.fryingTimeMax
                    });

                if (fryingTimer >= fryingRecipeSO.fryingTimeMax)
                {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(
                        fryingRecipeSO.output, this);

                    state = State.Fried;
                    burningTimer = 0f;

                    burningRecipeSO =
                        GetBurningRecipeSOWithInput(
                            GetKitchenObject().GetKitchenObjectSO());

                    OnStateChanged?.Invoke(this,
                        new OnStateChangedEventArgs { state = state });

                    OnCooked?.Invoke(this, EventArgs.Empty);

                    StopStoveSound();
                }
                break;

            case State.Fried:
                burningTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this,
                    new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized =
                            burningTimer / burningRecipeSO.burningTimerMax
                    });

                if (burningTimer >= burningRecipeSO.burningTimerMax)
                {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(
                        burningRecipeSO.output, this);

                    state = State.Burned;

                    OnStateChanged?.Invoke(this,
                        new OnStateChangedEventArgs { state = state });

                    OnProgressChanged?.Invoke(this,
                        new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });

                    StopStoveSound();
                }
                break;
        }
    }

    // =====================================================
    // =================== INTERACT =======================
    // =====================================================

    public override void Interact(Player player)
    {
        // ⏸ Không cho tương tác khi Pause
        if (GamePauseManager.Instance != null &&
            GamePauseManager.Instance.IsPaused())
            return;

        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject() &&
                HasRecipeWithInput(
                    player.GetKitchenObject().GetKitchenObjectSO()))
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);

                fryingRecipeSO =
                    GetFryingRecipeSOWithInput(
                        GetKitchenObject().GetKitchenObjectSO());

                state = State.Frying;
                fryingTimer = 0f;

                OnStateChanged?.Invoke(this,
                    new OnStateChangedEventArgs { state = state });

                OnProgressChanged?.Invoke(this,
                    new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });

                PlayStoveSound();
            }
        }
        else
        {
            if (player.HasKitchenObject() &&
                player.GetKitchenObject()
                    .TryGetPlate(out PlateKitchenObject plate))
            {
                if (plate.TryAddIngredient(
                    GetKitchenObject().GetKitchenObjectSO()))
                {
                    GetKitchenObject().DestroySelf();
                    ResetToIdle();
                }
            }
            else if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                ResetToIdle();
            }
        }
    }

    // =====================================================
    // =================== AUDIO ==========================
    // =====================================================

    private void PlayStoveSound()
    {
        if (stoveAudioSource == null) return;
        if (!stoveAudioSource.isPlaying)
            stoveAudioSource.Play();
    }

    private void StopStoveSound()
    {
        if (stoveAudioSource == null) return;
        if (stoveAudioSource.isPlaying)
            stoveAudioSource.Stop();
    }

    // =====================================================
    // =================== HELPERS ========================
    // =====================================================

    private void ResetToIdle()
    {
        state = State.Idle;
        StopStoveSound();

        OnStateChanged?.Invoke(this,
            new OnStateChangedEventArgs { state = state });

        OnProgressChanged?.Invoke(this,
            new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
    }

    private bool HasRecipeWithInput(KitchenObjectSO input)
        => GetFryingRecipeSOWithInput(input) != null;

    private FryingRecipeSO GetFryingRecipeSOWithInput(
        KitchenObjectSO input)
    {
        foreach (var f in fryingRecipeSOArray)
            if (f.input == input) return f;
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(
        KitchenObjectSO input)
    {
        foreach (var b in burningRecipeSOArray)
            if (b.input == input) return b;
        return null;
    }

    private void OnGameOver(object sender, EventArgs e)
    {
        StopStoveSound();
    }
}
