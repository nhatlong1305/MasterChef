using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverOutlineUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Outline Component")]
    [SerializeField] private Outline outline;

    [Header("Settings")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float animSpeed = 10f;
    [SerializeField] private float outlineMax = 5f;

    private Vector3 originalScale;
    private float originalOutline;

    private void Awake()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        originalScale = transform.localScale;
        originalOutline = outline.effectDistance.x;

        // Bắt đầu mờ outline
        outline.effectDistance = new Vector2(0, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(HoverEffect(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(HoverEffect(false));
    }


    private IEnumerator HoverEffect(bool isHover)
    {
        Vector3 targetScale = isHover ? originalScale * hoverScale : originalScale;
        float targetOutline = isHover ? outlineMax : 0f;

        while (true)
        {
            // Scale animation
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animSpeed);

            // Outline animation
            float newOutline = Mathf.Lerp(outline.effectDistance.x, targetOutline, Time.deltaTime * animSpeed);
            outline.effectDistance = new Vector2(newOutline, newOutline);

            // Dừng khi gần đạt mục tiêu
            if (Vector3.Distance(transform.localScale, targetScale) < 0.001f &&
                Mathf.Abs(outline.effectDistance.x - targetOutline) < 0.01f)
            {
                transform.localScale = targetScale;
                outline.effectDistance = new Vector2(targetOutline, targetOutline);
                break;
            }

            yield return null;
        }
    }
}
