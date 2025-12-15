using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RecipeItemUI : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI recipeName;

    private Color normalColor = Color.white;
    private Color failedColor = new Color(1f, 0.4f, 0.4f);

    public RecipeSO RecipeSO { get; private set; }

    public void SetRecipe(RecipeSO recipeSO)
    {
        RecipeSO = recipeSO;
        recipeName.text = recipeSO.recipeName;
        background.color = normalColor;
    }

    public void PlayFailedEffect()
    {
        StartCoroutine(FailAnimation());
    }

    public void Init(Image background, TextMeshProUGUI recipeName)
    {
        this.background = background;
        this.recipeName = recipeName;
    }

    private IEnumerator FailAnimation()
    {
        background.color = failedColor;
        // Fade out
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            Color c = background.color;
            c.a = 1 - t;
            background.color = c;
            yield return null;
        }

        Destroy(gameObject);
    }
}
