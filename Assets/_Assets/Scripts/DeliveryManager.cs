using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region EVENT ARGS
public class RecipeDeliveredEventArgs : EventArgs
{
    public string recipeId;
}

public class RecipeFailedEventArgs : EventArgs
{
    public string recipeId;
    public RecipeSO recipe;
}
#endregion

public class DeliveryManager : MonoBehaviour
{
    // ================= EVENTS =================
    public event EventHandler OnRecipeSpawned;
    public event EventHandler<RecipeDeliveredEventArgs> OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler<RecipeFailedEventArgs> OnRecipeFailed;

    // ================= SINGLETON =================
    public static DeliveryManager Instance { get; private set; }

    // ================= CONFIG =================
    [SerializeField] private RecipeListSO recipeListSO;

    [Header("Game Duration")]
    [SerializeField] private float gameDuration = 180f; // ⭐ 3 phút (chuẩn)

    [Header("Spawn")]
    [SerializeField] private float spawnRecipeTimerMax = 4f;

    // ================= STATE =================
    private float gameTime;
    private float spawnRecipeTimer;

    private float spawnSpeedMultiplier = 1f;
    private float recipeTimeMultiplier = 1f;
    private int currentMaxRecipes = 2;

    private int successfulRecipesAmount;

    // ================= RECIPE GROUP =================
    private readonly List<RecipeSO> easyRecipes = new();
    private readonly List<RecipeSO> mediumRecipes = new();
    private readonly List<RecipeSO> hardRecipes = new();

    // ================= RECIPE INSTANCE =================
    [Serializable]
    public class RecipeInstance
    {
        public string id;
        public RecipeSO recipe;
        public float remainingTime;
        public bool isFailed;

        public RecipeInstance(RecipeSO recipe, float timeMultiplier)
        {
            this.recipe = recipe;
            remainingTime = recipe.recipeDuration / timeMultiplier;
            id = Guid.NewGuid().ToString();
            isFailed = false;
        }
    }

    private readonly List<RecipeInstance> waitingRecipes = new();

    // ================= LIFECYCLE =================
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var recipe in recipeListSO.recipeSOList)
        {
            int count = recipe.kitchenObjectSOList.Count;

            if (count <= 2)
                easyRecipes.Add(recipe);
            else if (count <= 4)
                mediumRecipes.Add(recipe);
            else
                hardRecipes.Add(recipe);
        }

    }


    private void Update()
    {
        // ❌ Tutorial đang chạy → không spawn
        if (TutorialManager.Instance != null &&
            TutorialManager.Instance.IsTutorialRunning)
            return;

        // ⏱ Game time
        gameTime += Time.deltaTime;
        gameTime = Mathf.Min(gameTime, gameDuration);

        UpdateDifficulty();

        // ⏳ Spawn recipe
        spawnRecipeTimer -= Time.deltaTime * spawnSpeedMultiplier;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = Mathf.Max(1.2f, spawnRecipeTimerMax / spawnSpeedMultiplier);

            if (waitingRecipes.Count < currentMaxRecipes)
                SpawnRandomRecipe();
        }

        // ⏰ Update recipe timers
        for (int i = waitingRecipes.Count - 1; i >= 0; i--)
        {
            RecipeInstance inst = waitingRecipes[i];

            if (inst.isFailed) continue;

            inst.remainingTime -= Time.deltaTime;

            if (inst.remainingTime <= 0f)
            {
                inst.isFailed = true;

                OnRecipeFailed?.Invoke(this, new RecipeFailedEventArgs
                {
                    recipeId = inst.id,
                    recipe = inst.recipe
                });

                StartCoroutine(RemoveRecipeAfterDelay(inst.id));
            }
        }
    }

    // ================= SPAWN =================
    private void SpawnRandomRecipe()
    {
        RecipeSO recipe = GetRecipeBasedOnDifficulty();
        if (recipe == null) return;

        RecipeInstance inst = new RecipeInstance(recipe, recipeTimeMultiplier);
        waitingRecipes.Add(inst);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator RemoveRecipeAfterDelay(string recipeId)
    {
        yield return new WaitForSeconds(0.7f);

        waitingRecipes.RemoveAll(r => r.id == recipeId);
    }

    // ================= DIFFICULTY =================
    private void UpdateDifficulty()
    {
        if (gameTime < 30f)
        {
            currentMaxRecipes = 2;
            spawnSpeedMultiplier = 1f;
            recipeTimeMultiplier = 1f;
        }
        else if (gameTime < 60f)
        {
            currentMaxRecipes = 3;
            spawnSpeedMultiplier = 1.3f;
            recipeTimeMultiplier = 1.2f;
        }
        else
        {
            currentMaxRecipes = 4;
            spawnSpeedMultiplier = 1.6f;
            recipeTimeMultiplier = 1.5f;
        }
    }



    private RecipeSO GetRecipeBasedOnDifficulty()
    {
        // ===== 0 – 30s: EASY ONLY =====
        if (gameTime < 30f)
        {
            return easyRecipes.Count > 0
                ? easyRecipes[UnityEngine.Random.Range(0, easyRecipes.Count)]
                : null;
        }

        // ===== 30 – 60s: MEDIUM ONLY =====
        if (gameTime < 60f)
        {
            return mediumRecipes.Count > 0
                ? mediumRecipes[UnityEngine.Random.Range(0, mediumRecipes.Count)]
                : easyRecipes.Count > 0
                    ? easyRecipes[UnityEngine.Random.Range(0, easyRecipes.Count)]
                    : null;
        }

        // ===== > 60s: RANDOM EASY / MEDIUM / HARD =====
        float rand = UnityEngine.Random.value;

        if (rand < 0.33f && easyRecipes.Count > 0)
            return easyRecipes[UnityEngine.Random.Range(0, easyRecipes.Count)];

        if (rand < 0.66f && mediumRecipes.Count > 0)
            return mediumRecipes[UnityEngine.Random.Range(0, mediumRecipes.Count)];

        if (hardRecipes.Count > 0)
            return hardRecipes[UnityEngine.Random.Range(0, hardRecipes.Count)];

        // Fallback an toàn
        if (mediumRecipes.Count > 0)
            return mediumRecipes[UnityEngine.Random.Range(0, mediumRecipes.Count)];

        return easyRecipes.Count > 0
            ? easyRecipes[UnityEngine.Random.Range(0, easyRecipes.Count)]
            : null;
    }



    // ================= DELIVERY =================
    public void DeliverRecipe(PlateKitchenObject plate)
    {
        if (TutorialManager.Instance != null &&
            TutorialManager.Instance.IsTutorialRunning)
            return;

        List<KitchenObjectSO> plateIngredients = plate.GetKitchenObjectSOList();

        for (int i = 0; i < waitingRecipes.Count; i++)
        {
            RecipeInstance inst = waitingRecipes[i];

            if (IsRecipeMatch(inst.recipe, plateIngredients))
            {
                successfulRecipesAmount++;

                int score = inst.recipe.GetRecipeScore();
                ScoreManager.Instance.AddRecipeScore(score);

                OnRecipeCompleted?.Invoke(this,
                    new RecipeDeliveredEventArgs { recipeId = inst.id });

                waitingRecipes.RemoveAt(i);

                OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        // ❌ Sai món
        OnRecipeFailed?.Invoke(this,
            new RecipeFailedEventArgs { recipeId = null, recipe = null });
    }

    // ================= MATCH =================
    private bool IsRecipeMatch(RecipeSO recipe, List<KitchenObjectSO> plate)
    {
        if (recipe.kitchenObjectSOList.Count != plate.Count) return false;

        foreach (var required in recipe.kitchenObjectSOList)
        {
            if (!plate.Contains(required))
                return false;
        }
        return true;
    }

    // ================= PUBLIC API =================
    public List<RecipeInstance> GetWaitingRecipes() => waitingRecipes;
    public int GetSuccessfulRecipesAmount() => successfulRecipesAmount;
    public RecipeInstance GetBestMatchingRecipe(
    List<KitchenObjectSO> plateIngredients)
    {
        RecipeInstance bestFullMatch = null;
        RecipeInstance bestPartialMatch = null;

        foreach (var inst in waitingRecipes)
        {
            RecipeSO recipe = inst.recipe;

            // ✅ Full match (ưu tiên cao nhất)
            if (recipe.kitchenObjectSOList.Count == plateIngredients.Count &&
                IsRecipeMatch(recipe, plateIngredients))
            {
                bestFullMatch = inst;
                break;
            }

            // ⚠ Partial match (đúng nguyên liệu nhưng thiếu)
            if (IsPartialMatch(recipe, plateIngredients))
            {
                if (bestPartialMatch == null ||
                    recipe.kitchenObjectSOList.Count <
                    bestPartialMatch.recipe.kitchenObjectSOList.Count)
                {
                    bestPartialMatch = inst;
                }
            }
        }

        return bestFullMatch ?? bestPartialMatch;
    }
    private bool IsPartialMatch(
        RecipeSO recipe,
        List<KitchenObjectSO> plate)
    {
        if (plate.Count == 0) return false;
        if (plate.Count > recipe.kitchenObjectSOList.Count) return false;

        foreach (var ing in plate)
        {
            if (!recipe.kitchenObjectSOList.Contains(ing))
                return false;
        }

        return true;
    }

}
