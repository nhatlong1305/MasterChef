using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSimpleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer)
        {
            if(child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            Transform iconTranform = Instantiate(iconTemplate, iconContainer);
            iconTranform.gameObject.SetActive(true);
            iconTranform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }
}
