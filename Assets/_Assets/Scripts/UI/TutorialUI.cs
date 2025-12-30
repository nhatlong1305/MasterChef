using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        TutorialManager.Instance.OnStepChanged += TutorialManager_OnStepChanged;
        TutorialManager.Instance.OnTutorialCompleted += TutorialManager_OnTutorialCompleted;

        Hide();
    }

    private void TutorialManager_OnStepChanged(object sender, TutorialStepSO step)
    {
        Show();
        titleText.text = step.title;
        descriptionText.text = step.description;
    }

    private void TutorialManager_OnTutorialCompleted()
    {
        StartCoroutine(ShowFinalHint());
    }

    private IEnumerator ShowFinalHint()
    {
        Show();
        titleText.text = "Game Start";
        descriptionText.text = "Note: If you make a mistake, throw it in the trash.";

        yield return new WaitForSeconds(4f);

        Hide();
    }

    private void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
