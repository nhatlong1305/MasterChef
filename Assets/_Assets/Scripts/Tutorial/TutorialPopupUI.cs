using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupUI : MonoBehaviour
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private CanvasGroup canvasGroup;

    private const string SKIP_KEY = "SkipTutorial";

    private void Awake()
    {
      
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        yesButton.GetComponent<UIButtonEffect>()
            .SetClickAction(OnYesClicked);

        noButton.GetComponent<UIButtonEffect>()
            .SetClickAction(OnNoClicked);

        if (PlayerPrefs.GetInt(SKIP_KEY, 0) == 1)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void Hide()
    {

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        gameObject.SetActive(false);
    }

    private void OnYesClicked()
    {
        TutorialManager.Instance.StartTutorial();
        Hide();
    }

    private void OnNoClicked()
    {
        PlayerPrefs.SetInt(SKIP_KEY, 1);
        PlayerPrefs.Save();

        TutorialManager.Instance.SkipTutorial();
        Hide();
    }
}
