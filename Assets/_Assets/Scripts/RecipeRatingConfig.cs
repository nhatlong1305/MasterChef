using UnityEngine;

public class RecipeRatingConfig : MonoBehaviour
{
    public static RecipeRatingConfig Instance { get; private set; }

    [Header("Final Star Rating Based on TOTAL SCORE")]
    public int star1Threshold = 50;
    public int star2Threshold = 120;
    public int star3Threshold = 200;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public int GetFinalStarRanking(int totalScore)
    {
        if (totalScore >= star3Threshold) return 3;
        if (totalScore >= star2Threshold) return 2;
        if (totalScore >= star1Threshold) return 1;
        return 0;
    }
}
