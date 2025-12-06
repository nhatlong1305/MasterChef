using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;

    private void Start()
    {
        gameObject.SetActive(false); 

       
        KitchenGameManager.Instance.OnGameOver += Instance_OnGameOver;
    }

    private void Instance_OnGameOver(object sender, System.EventArgs e)
    {
        recipesDeliveredText.text =
            DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();

        gameObject.SetActive(true); 
    }
}
