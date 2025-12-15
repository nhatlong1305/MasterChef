using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarSimpleAnimator : MonoBehaviour
{
    [SerializeField] private Image starImage;
    [SerializeField] private Sprite starOnSprite;
    [SerializeField] private Sprite starOffSprite;

    private Vector3 startScale = Vector3.one * 0.7f;
    private Vector3 finalScale = new Vector3(2.87f, 2.87f, 2.87f);

    public void ResetStar()
    {
        starImage.sprite = starOffSprite;

        var c = starImage.color;
        c.a = 0.2f;  // mờ nhẹ
        starImage.color = c;

        transform.localScale = startScale;
    }

    public IEnumerator Play()
    {
        starImage.sprite = starOnSprite;

        float t = 0f;

        // ⭐ Scale từ 0.7 → 2.87 và giữ nguyên ⭐
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;

            // Fade alpha
            Color c = starImage.color;
            c.a = Mathf.Lerp(0.2f, 1f, t);
            starImage.color = c;

            // Scale
            transform.localScale = Vector3.Lerp(startScale, finalScale, t);

            yield return null;
        }

        // ⭐ GIỮ NGUYÊN KÍCH THƯỚC SAU KHI SCALE ⭐
        transform.localScale = finalScale;
    }
}
