using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarSimpleAnimator : MonoBehaviour
{
    [SerializeField] private Image starImage;
    [SerializeField] private Sprite starOnSprite;
    [SerializeField] private Sprite starOffSprite;

    private Vector3 startScale = Vector3.one * 0.7f;
    private Vector3 finalScale = new Vector3(1.87f, 1.87f, 1.87f);

    public void ResetStar()
    {
        starImage.sprite = starOffSprite;

        var c = starImage.color;
        c.a = 0.2f;  
        starImage.color = c;

        transform.localScale = startScale;
    }

    public IEnumerator Play()
    {
        starImage.color = new Color(1f, 0.84f, 0.2f, 0.2f);

        float t = 0f;

       
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;

            Color c = starImage.color;
            c.a = Mathf.Lerp(0.2f, 1f, t);
            starImage.color = c;

          
            transform.localScale = Vector3.Lerp(startScale, finalScale, t);

            yield return null;
        }

      
        transform.localScale = finalScale;
    }
}
