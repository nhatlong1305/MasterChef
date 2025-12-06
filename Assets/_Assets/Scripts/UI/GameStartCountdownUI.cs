using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        // Luôn tắt khi bắt đầu game
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (TutorialManager.Instance.IsTutorialRunning)
        {
            Hide();
            return;
        }

        if (KitchenGameManager.Instance.IsCountdownToStartActive())
            Show();
        else
            Hide();
    }


    private void Update()
    {
        if (!KitchenGameManager.Instance.IsCountdownToStartActive())
            return;

        float time = KitchenGameManager.Instance.GetCountdownToStartTime();
        countdownText.text = Mathf.Ceil(time).ToString();
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
