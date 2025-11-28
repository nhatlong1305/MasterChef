using UnityEngine;
using UnityEngine.UI;

public class GamePlayingBlockUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    private void Update()
    {
        timerImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
