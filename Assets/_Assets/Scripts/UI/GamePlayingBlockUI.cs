using UnityEngine;
using UnityEngine.UI;

public class GamePlayingBlockUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += Instance_OnStateChanged;
        Hide();
    }

    private void Instance_OnStateChanged(object sender, System.EventArgs e)
    {

        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialRunning)
        {
            Hide();
            return;
        }

        if (KitchenGameManager.Instance.IsGamePlaying())
            Show();
        else
            Hide();
    }


    private void Update()
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        timerImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}