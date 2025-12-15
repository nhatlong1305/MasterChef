using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeDeliveredEventArgs : EventArgs
{
    public string recipeId;
}

public class RecipeFailedEventArgs : EventArgs
{
    public string recipeId;
    public RecipeSO recipe;
}

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler<RecipeFailedEventArgs> OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;

    private int successfulRecipesAmount = 0;

    [SerializeField] private float gameDuration = 300f;
    private float gameTime = 0f;

    private float spawnSpeedMultiplier = 1f;
    private float recipeTimeMultiplier = 1f;
    private int currentMaxRecipes = 2;

    private List<RecipeSO> easyRecipes = new List<RecipeSO>();
    private List<RecipeSO> mediumRecipes = new List<RecipeSO>();
    private List<RecipeSO> hardRecipes = new List<RecipeSO>();


    [Serializable]
    public class RecipeInstance
    {
        public string id;
        public RecipeSO recipe;
        public float remainingTime;

        public RecipeInstance(RecipeSO recipe)
        {
            this.recipe = recipe;
            this.remainingTime = recipe.recipeDuration;
            this.id = Guid.NewGuid().ToString();
        }
    }

    private List<RecipeInstance> waitingRecipes = new List<RecipeInstance>();


    private void Awake()
    {
        Instance = this;

        foreach (var recipe in recipeListSO.recipeSOList)
        {
            int count = recipe.kitchenObjectSOList.Count;

            if (count >= 2 && count <= 3) easyRecipes.Add(recipe);
            if (count >= 2 && count <= 4) mediumRecipes.Add(recipe);
            if (count >= 2 && count <= 5) hardRecipes.Add(recipe);
        }
    }


    private void Update()
    {
        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialRunning)
            return;

        gameTime += Time.deltaTime;
        if (gameTime > gameDuration) gameTime = gameDuration;

        UpdateDifficulty();

        spawnRecipeTimer -= Time.deltaTime * spawnSpeedMultiplier;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipes.Count < currentMaxRecipes)
                SpawnRandomRecipe();
        }

        for (int i = waitingRecipes.Count - 1; i >= 0; i--)
        {
            RecipeInstance inst = waitingRecipes[i];

            inst.remainingTime -= Time.deltaTime;

            if (inst.remainingTime <= 0f)
            {
                OnRecipeFailed?.Invoke(this, new RecipeFailedEventArgs
                {
                    recipeId = inst.id,
                    recipe = inst.recipe
                });

                StartCoroutine(RemoveRecipeAfterDelay(inst.id));
            }
        }
    }


    private IEnumerator RemoveRecipeAfterDelay(string recipeId)
    {
        yield return new WaitForSeconds(0.7f); 

        for (int i = 0; i < waitingRecipes.Count; i++)
        {
            if (waitingRecipes[i].id == recipeId)
            {
                waitingRecipes.RemoveAt(i);
                break;
            }
        }
    }


    private void UpdateDifficulty()
    {
        float t = gameTime / gameDuration;

        if (t < 0.2f)
        {
            currentMaxRecipes = 2;
            spawnSpeedMultiplier = 1f;
            recipeTimeMultiplier = 1f;
        }
        else if (t < 0.6f)
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
        float t = gameTime / gameDuration;

        if (t < 0.2f && easyRecipes.Count > 0)
            return easyRecipes[UnityEngine.Random.Range(0, easyRecipes.Count)];

        if (t < 0.6f)
        {
            if (UnityEngine.Random.value < 0.7f && easyRecipes.Count > 0)
                return easyRecipes[UnityEngine.Random.Range(0, easyRecipes.Count)];

            if (mediumRecipes.Count > 0)
                return mediumRecipes[UnityEngine.Random.Range(0, mediumRecipes.Count)];
        }

        if (UnityEngine.Random.value < 0.3f && mediumRecipes.Count > 0)
            return mediumRecipes[UnityEngine.Random.Range(0, mediumRecipes.Count)];

        if (hardRecipes.Count > 0)
            return hardRecipes[UnityEngine.Random.Range(0, hardRecipes.Count)];

        if (easyRecipes.Count > 0)
            return easyRecipes[UnityEngine.Random.Range(0, easyRecipes.Count)];

        Debug.LogError("NO RECIPES AVAILABLE!");
        return null;
    }


    //      PUBLIC API
    

    public List<RecipeInstance> GetWaitingRecipes() => waitingRecipes;

    public int GetSuccessfulRecipesAmount() => successfulRecipesAmount;


    public RecipeInstance GetBestMatchingRecipe(List<KitchenObjectSO> plateIngredients)
    {
        RecipeInstance bestFull = null;
        RecipeInstance bestPartial = null;

        foreach (var inst in waitingRecipes)
        {
            RecipeSO recipe = inst.recipe;

            if (recipe.kitchenObjectSOList.Count == plateIngredients.Count &&
                IsRecipeMatch(recipe, plateIngredients))
            {
                bestFull = inst;
                break;
            }

            if (IsPartialMatch(recipe, plateIngredients))
            {
                if (bestPartial == null ||
                    recipe.kitchenObjectSOList.Count < bestPartial.recipe.kitchenObjectSOList.Count)
                {
                    bestPartial = inst;
                }
            }
        }

        return bestFull ?? bestPartial;
    }


    private bool IsPartialMatch(RecipeSO recipe, List<KitchenObjectSO> plate)
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


    private void SpawnRandomRecipe()
    {
        RecipeSO recipe = GetRecipeBasedOnDifficulty();
        if (recipe == null) return;

        RecipeInstance inst = new RecipeInstance(recipe);
        inst.remainingTime = recipe.recipeDuration / recipeTimeMultiplier;

        waitingRecipes.Add(inst);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }


    public void DeliverRecipe(PlateKitchenObject plate)
    {
        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialRunning)
            return;

        List<KitchenObjectSO> plateIngredients = plate.GetKitchenObjectSOList();

        for (int i = 0; i < waitingRecipes.Count; i++)
        {
            RecipeInstance inst = waitingRecipes[i];
            RecipeSO recipe = inst.recipe;

            if (recipe.kitchenObjectSOList.Count == plateIngredients.Count &&
                IsRecipeMatch(recipe, plateIngredients))
            {
                successfulRecipesAmount++;

                int score = recipe.GetRecipeScore();
                ScoreManager.Instance.AddRecipeScore(score);

                OnRecipeCompleted?.Invoke(this, new RecipeDeliveredEventArgs
                {
                    recipeId = inst.id
                });

                waitingRecipes.RemoveAt(i);

                OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

                return;
            }
        }

        // Fail
        OnRecipeFailed?.Invoke(this, new RecipeFailedEventArgs
        {
            recipeId = null,
            recipe = null
        });
    }



    private bool IsRecipeMatch(RecipeSO recipe, List<KitchenObjectSO> plate)
    {
        foreach (var required in recipe.kitchenObjectSOList)
        {
            bool found = false;

            foreach (var ing in plate)
            {
                if (required == ing)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;
        }
        return true;
    }
}
