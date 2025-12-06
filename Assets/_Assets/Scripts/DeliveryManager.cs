using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public bool HasDelivered { get; private set; }

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;

    private int successfulRecipesAmount = 0;

    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
   
        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialRunning)
            return;

        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipesMax)
            {
                SpawnRandomRecipe();
            }
        }
    }

    private void SpawnRandomRecipe()
    {
        if (recipeListSO == null || recipeListSO.recipeSOList.Count == 0)
        {
      
            return;
        }

        RecipeSO newRecipe = recipeListSO.recipeSOList[
            UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)
        ];

        waitingRecipeSOList.Add(newRecipe);

        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
  
        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialRunning)
            return;

        if (plateKitchenObject == null)
        {
            Debug.LogWarning("DeliveryManager: plateKitchenObject null.");
            OnRecipeFailed?.Invoke(this, EventArgs.Empty);
            return;
        }

        List<KitchenObjectSO> plateIngredients = plateKitchenObject.GetKitchenObjectSOList();

        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO recipe = waitingRecipeSOList[i];

            if (recipe.kitchenObjectSOList.Count != plateIngredients.Count)
                continue;

            if (IsRecipeMatch(recipe, plateIngredients))
            {
                successfulRecipesAmount++;
                waitingRecipeSOList.RemoveAt(i);

                HasDelivered = true;

                OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

                return;
            }
        }

        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    private bool IsRecipeMatch(RecipeSO recipe, List<KitchenObjectSO> plateIngredients)
    {
        foreach (KitchenObjectSO recipeIngredient in recipe.kitchenObjectSOList)
        {
            bool found = false;

            foreach (KitchenObjectSO plateIngredient in plateIngredients)
            {
                if (plateIngredient == recipeIngredient)
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

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount()
    {
        return successfulRecipesAmount;
    }

    public void ResetDeliveryFlags()
    {
        HasDelivered = false;

      
        waitingRecipeSOList.Clear();
    }
}
