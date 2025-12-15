using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
    [Header("Ingredients")]
    public List<KitchenObjectSO> kitchenObjectSOList;

    public string recipeName;

    
    public int GetRecipeScore()
    {
        int total = 0;
        foreach (var ingredient in kitchenObjectSOList)
        {
            if (ingredient != null)
                total += ingredient.ingredientScore;
        }
        return total;
    }

    [Header("Timer")]
    [Range(1, 120)]
    public float recipeDuration = 30f;   
}
