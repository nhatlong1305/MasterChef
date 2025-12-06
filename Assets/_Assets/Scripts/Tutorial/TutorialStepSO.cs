using UnityEngine;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Game/Tutorial Step")]
public class TutorialStepSO : ScriptableObject
{
    

    public string title;
    public string description;

    public TutorialConditionType conditionType;


    public string targetCounterName;

    public bool requireSpecificIngredient;
    public KitchenObjectSO targetIngredient;
}


