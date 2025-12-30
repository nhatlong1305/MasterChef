using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private TextMeshProUGUI totalScoreText;

    [Header("Stars")]
    [SerializeField] private Image[] starImages;

    private void Start()
    {
        KitchenGameManager.Instance.OnGameOver += KitchenGameManager_OnGameOver;
        Hide();
    }

    private void OnDestroy()
    {
        if (KitchenGameManager.Instance != null)
            KitchenGameManager.Instance.OnGameOver -= KitchenGameManager_OnGameOver;
    }


    private void KitchenGameManager_OnGameOver(object sender, System.EventArgs e)
    {
        Show();

        
        recipesDeliveredText.text =
            DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();

      
        int totalScore = ScoreManager.Instance.GetTotalScore();
        totalScoreText.text = "Score: " + totalScore;

        int starCount =
            RecipeRatingConfig.Instance.GetFinalStarRanking(totalScore);

        UpdateStars(starCount);
    }


    private void UpdateStars(int starCount)
    {
        StopAllCoroutines();
        StartCoroutine(StarSequence(starCount));
    }

    private IEnumerator StarSequence(int starCount)
    {
        foreach (var img in starImages)
        {
            var anim = img.GetComponent<StarSimpleAnimator>();
            if (anim != null)
                anim.ResetStar();
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < starCount && i < starImages.Length; i++)
        {
            var anim = starImages[i].GetComponent<StarSimpleAnimator>();
            if (anim != null)
                yield return StartCoroutine(anim.Play());

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("GameMenuScenes");
    }


    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
