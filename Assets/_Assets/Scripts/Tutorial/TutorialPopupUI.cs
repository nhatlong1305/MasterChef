using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupUI : MonoBehaviour
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private const string SKIP_KEY = "SkipTutorial";

    private void Start()
    {
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);

        // Nếu restart → tự ẩn popup
        if (PlayerPrefs.GetInt(SKIP_KEY, 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnYesClicked()
    {
        TutorialManager.Instance.SkipTutorial();
        gameObject.SetActive(false);
    }

    private void OnNoClicked()
    {
        TutorialManager.Instance.StartTutorial();
        gameObject.SetActive(false);
    }
}
