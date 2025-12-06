using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeUpdated;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeUpdated;

        // Ẩn UI khi vào tutorial
        TutorialManager.Instance.OnStepChanged += OnTutorialStep;
        TutorialManager.Instance.OnTutorialCompleted += OnTutorialCompleted;

        UpdateVisual();
        Hide(); // Ẩn UI từ đầu vì tutorial đang chạy
    }

    // Ẩn UI khi đang ở bất kỳ bước tutorial nào
    private void OnTutorialStep(object sender, TutorialStepSO step)
    {
        Hide();
    }

    // Hiện UI khi tutorial kết thúc
    private void OnTutorialCompleted()
    {
        Show();
        UpdateVisual();  // Cập nhật lại danh sách sau khi hiện UI
    }

    private void DeliveryManager_OnRecipeUpdated(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Nếu vẫn trong tutorial → không render UI
        if (TutorialManager.Instance.IsTutorialRunning)
        {
            Hide();
            return;
        }

        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSimpleUI>().SetRecipeSO(recipeSO);
        }
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
