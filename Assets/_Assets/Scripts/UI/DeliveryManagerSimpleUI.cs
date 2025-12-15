using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSimpleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    [Header("Timer UI")]
    [SerializeField] private Slider timerSlider;

    private DeliveryManager.RecipeInstance recipeInstance;

    public string RecipeId { get; private set; }

    [SerializeField] private CanvasGroup highlightGroup;


    public void SetRecipeInstance(DeliveryManager.RecipeInstance instance)
    {
        recipeInstance = instance;

        RecipeId = instance.id;

        recipeNameText.text = instance.recipe.recipeName;

        // Reset icon list
        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        // Spawn icons
        foreach (KitchenObjectSO kitchenObjectSO in instance.recipe.kitchenObjectSOList)
        {
            Transform iconTranform = Instantiate(iconTemplate, iconContainer);
            iconTranform.gameObject.SetActive(true);
            iconTranform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }

        // Setup Timer UI
        timerSlider.maxValue = instance.recipe.recipeDuration;
        timerSlider.value = instance.recipe.recipeDuration;
    }

    private void Update()
    {
        if (recipeInstance == null) return;

        timerSlider.value = recipeInstance.remainingTime;
    }

    public void SetHighlighted(bool isOn)
    {
        if (highlightGroup == null) return;

        highlightGroup.alpha = isOn ? 1f : 0.5f;
        transform.localScale = isOn ? Vector3.one * 1.05f : Vector3.one;
    }


}
