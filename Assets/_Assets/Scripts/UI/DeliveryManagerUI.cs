using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        //  Delivery 
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
            DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
            DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        }

        //  Game State 
        if (KitchenGameManager.Instance != null)
        {
            KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        }

        //  Plate 
        PlateKitchenObject.OnAnyIngredientAdded += Plate_OnAnyIngredientAdded;

        //  Tutorial 
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnStepChanged += OnTutorialStep;
            TutorialManager.Instance.OnTutorialCompleted += OnTutorialCompleted;
        }

        UpdateVisual();
        Hide(); 
    }


    private void OnDestroy()
    {
        //  Delivery 
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSpawned -= DeliveryManager_OnRecipeSpawned;
            DeliveryManager.Instance.OnRecipeFailed -= DeliveryManager_OnRecipeFailed;
            DeliveryManager.Instance.OnRecipeCompleted -= DeliveryManager_OnRecipeCompleted;
        }

        //  Game State 
        if (KitchenGameManager.Instance != null)
        {
            KitchenGameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
        }

        //  Plate
        PlateKitchenObject.OnAnyIngredientAdded -= Plate_OnAnyIngredientAdded;

        //  Tutorial 
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnStepChanged -= OnTutorialStep;
            TutorialManager.Instance.OnTutorialCompleted -= OnTutorialCompleted;
        }
    }



    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (KitchenGameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
            return;
        }

        if (KitchenGameManager.Instance.IsGamePlaying())
        {
            Show();
            UpdateVisual();
            return;
        }

        if (KitchenGameManager.Instance.IsGameOver())
        {
            Hide();
            return;
        }
    }



    private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, EventArgs e)
    {
        if (e is RecipeDeliveredEventArgs args)
        {
            RemoveRecipeUI(args.recipeId);
        }
    }

    private void DeliveryManager_OnRecipeFailed(object sender, RecipeFailedEventArgs e)
    {
        if (!gameObject.activeInHierarchy) return;
        if (string.IsNullOrEmpty(e.recipeId)) return;

        ShowFailedAnimation(e.recipeId);
        StartCoroutine(RemoveRecipeAfterDelay(e.recipeId));
    }

    private IEnumerator RemoveRecipeAfterDelay(string recipeId)
    {
        yield return new WaitForSeconds(0.7f);
        RemoveRecipeUI(recipeId);
    }



    private void Plate_OnAnyIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        PlateKitchenObject plate = sender as PlateKitchenObject;
        if (plate == null) return;

        List<KitchenObjectSO> ingredients = plate.GetKitchenObjectSOList();
        var match = DeliveryManager.Instance.GetBestMatchingRecipe(ingredients);

        if (match != null)
            HighlightRecipe(match.id);
        else
            ClearHighlight();
    }

    private void UpdateVisual()
    {
        if (TutorialManager.Instance != null &&
            TutorialManager.Instance.IsTutorialRunning)
        {
            Hide();
            return;
        }

        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (var inst in DeliveryManager.Instance.GetWaitingRecipes())
        {
            Transform obj = Instantiate(recipeTemplate, container);
            obj.gameObject.SetActive(true);

            var simpleUI = obj.GetComponent<DeliveryManagerSimpleUI>();
            simpleUI.SetRecipeInstance(inst);


            var itemUI = obj.gameObject.AddComponent<RecipeItemUI>();
            Image bg = obj.Find("Background").GetComponent<Image>();
            TextMeshProUGUI nameText =
                obj.Find("Background/RecipesNameText").GetComponent<TextMeshProUGUI>();

            itemUI.Init(bg, nameText);
            itemUI.SetRecipe(inst.recipe);
        }
    }


    private void RemoveRecipeUI(string recipeId)
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;

            var ui = child.GetComponent<DeliveryManagerSimpleUI>();
            if (ui != null && ui.RecipeId == recipeId)
            {
                Destroy(child.gameObject);
                return;
            }
        }
    }


    private void ShowFailedAnimation(string recipeId)
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;
            if (!child.gameObject.activeInHierarchy) continue;

            var ui = child.GetComponent<DeliveryManagerSimpleUI>();
            if (ui != null && ui.RecipeId == recipeId)
            {
                var item = child.GetComponent<RecipeItemUI>();
                if (item != null)
                {
                    item.PlayFailedEffect();
                }
                break;
            }
        }
    }



    private void HighlightRecipe(string recipeId)
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;

            var ui = child.GetComponent<DeliveryManagerSimpleUI>();
            if (ui != null)
            {
                ui.SetHighlighted(ui.RecipeId == recipeId);
            }
        }
    }

    private void ClearHighlight()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;

            var ui = child.GetComponent<DeliveryManagerSimpleUI>();
            if (ui != null)
            {
                ui.SetHighlighted(false);
            }
        }
    }



    private void OnTutorialStep(object sender, TutorialStepSO step)
    {
        Hide();
    }

    private void OnTutorialCompleted()
    {
        Show();
        UpdateVisual();
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
