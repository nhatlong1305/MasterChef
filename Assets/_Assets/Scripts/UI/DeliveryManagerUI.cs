using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;


        PlateKitchenObject.OnAnyIngredientAdded += Plate_OnAnyIngredientAdded;

        TutorialManager.Instance.OnStepChanged += OnTutorialStep;
        TutorialManager.Instance.OnTutorialCompleted += OnTutorialCompleted;

        UpdateVisual();
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e)
    {
        // Khi đang countdown → ẩn UI
        if (KitchenGameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
            return;
        }

        // Khi bắt đầu chơi → hiện UI
        if (KitchenGameManager.Instance.IsGamePlaying())
        {
            Show();
            UpdateVisual();
            return;
        }

        // Khi game over → ẩn UI
        if (KitchenGameManager.Instance.IsGameOver())
        {
            Hide();
            return;
        }
    }



    // ========================================
    // EVENTS
    // ========================================
    private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }


    private void DeliveryManager_OnRecipeFailed(object sender, RecipeFailedEventArgs e)
    {
        if (!gameObject.activeInHierarchy) return;
        if (!string.IsNullOrEmpty(e.recipeId))
        {
            ShowFailedAnimation(e.recipeId);
            StartCoroutine(RefreshUIAfterDelay(e.recipeId));
        }
       
    }


    private IEnumerator RefreshUIAfterDelay(string recipeId)
    {
        yield return new WaitForSeconds(0.7f); 
        RemoveRecipeUI(recipeId);
    }


    private void DeliveryManager_OnRecipeCompleted(object sender, EventArgs e)
    {
        if (e is RecipeDeliveredEventArgs args)
            RemoveRecipeUI(args.recipeId);
    }


    private void Plate_OnAnyIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        PlateKitchenObject plate = sender as PlateKitchenObject;
        if (plate == null) return;

        var ingredients = plate.GetKitchenObjectSOList();
        var match = DeliveryManager.Instance.GetBestMatchingRecipe(ingredients);

        if (match != null)
            HighlightRecipe(match.id);
        else
            ClearHighlight();
    }



    // FAIL ANIMATION
    public void ShowFailedAnimation(string recipeId)
    {
        // UI cha đang bị ẩn → không chạy animation
        if (!gameObject.activeInHierarchy) return;

        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;
            if (!child.gameObject.activeInHierarchy) continue;

            var ui = child.GetComponent<DeliveryManagerSimpleUI>();
            if (ui == null) continue;

            if (ui.RecipeId == recipeId)
            {
                var item = child.GetComponent<RecipeItemUI>();

                if (item != null && item.gameObject.activeInHierarchy)
                {
                    item.PlayFailedEffect();
                }

                break;
            }
        }
    }



    // UI UPDATE
    private void UpdateVisual()
    {
        if (TutorialManager.Instance.IsTutorialRunning)
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

            obj.GetComponent<DeliveryManagerSimpleUI>().SetRecipeInstance(inst);

            var itemUI = obj.gameObject.AddComponent<RecipeItemUI>();
            Image bg = obj.Find("Background").GetComponent<Image>();
            TextMeshProUGUI nameText = obj.Find("Background/RecipesNameText").GetComponent<TextMeshProUGUI>();

            itemUI.Init(bg, nameText);
            itemUI.SetRecipe(inst.recipe);
        }
    }


    // REMOVE UI
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


    // HIGHLIGHT
    public void HighlightRecipe(string recipeId)
    {
        if (!gameObject.activeInHierarchy) return;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);

            if (child == null) continue;
            if (child == recipeTemplate) continue;
            if (!child.gameObject.activeInHierarchy) continue;

            var ui = child.GetComponent<DeliveryManagerSimpleUI>();
            if (ui == null) continue;

            ui.SetHighlighted(ui.RecipeId == recipeId);
        }
    }


    public void ClearHighlight()
    {
        if (!gameObject.activeInHierarchy) return;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);

            if (child == null) continue;
            if (child == recipeTemplate) continue;

            var ui = child.GetComponent<DeliveryManagerSimpleUI>();
            if (ui == null) continue;

            ui.SetHighlighted(false);
        }
    }


    // TUTORIAL
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
