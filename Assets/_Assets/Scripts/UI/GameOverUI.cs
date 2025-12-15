using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    [Header("Stars")]
    [SerializeField] private Image[] starImages;  
    [SerializeField] private Sprite starOnSprite;
    [SerializeField] private Sprite starOffSprite;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameOver())
        {
            Show();

            recipesDeliveredText.text =
                DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();

            int totalScore = ScoreManager.Instance.GetTotalScore();
            totalScoreText.text = "Score: " + totalScore;

            int starCount = RecipeRatingConfig.Instance.GetFinalStarRanking(totalScore);

            UpdateStars(starCount);
        }
        else
        {
            Hide();
        }
    }

    private void UpdateStars(int starCount)
    {
        StopAllCoroutines();
        StartCoroutine(SimpleStarSequence(starCount));
    }

    private IEnumerator SimpleStarSequence(int starCount)
    {
        // reset sao
        foreach (var img in starImages)
        {
            img.GetComponent<StarSimpleAnimator>().ResetStar();
        }

        yield return new WaitForSeconds(0.2f);

        // bật từng sao 1
        for (int i = 0; i < starCount; i++)
        {
            yield return StartCoroutine(
                starImages[i].GetComponent<StarSimpleAnimator>().Play()
            );

            yield return new WaitForSeconds(0.1f); // delay nhẹ giữa các sao
        }
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
