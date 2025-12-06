using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupUI : MonoBehaviour
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void Start()
    {
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
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
