using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButtonEffect : MonoBehaviour
{
    private Vector3 normalScale;
    private Vector3 hoverScale = Vector3.one * 1.1f;
    private Vector3 clickScale = Vector3.one * 0.9f;

    private System.Action clickAction;

    private void Awake()
    {
        normalScale = transform.localScale;

        EventTriggerListener listener = gameObject.AddComponent<EventTriggerListener>();

        listener.onEnter = () =>
        {
            if (!gameObject.activeInHierarchy) return;
            StopAllCoroutines();
            StartCoroutine(ScaleTo(hoverScale, 0.12f));
        };

        listener.onExit = () =>
        {
            if (!gameObject.activeInHierarchy) return;
            StopAllCoroutines();
            StartCoroutine(ScaleTo(normalScale, 0.12f));
        };

        listener.onClick = () =>
        {
            if (!gameObject.activeInHierarchy) return;
            StopAllCoroutines();
            StartCoroutine(ClickAnim());
        };
    }

    public void SetClickAction(System.Action action)
    {
        clickAction = action;
    }

    private IEnumerator ClickAnim()
    {
        yield return ScaleTo(clickScale, 0.08f);
        yield return ScaleTo(normalScale, 0.12f);

        clickAction?.Invoke(); 
    }

    private IEnumerator ScaleTo(Vector3 target, float time)
    {
        Vector3 start = transform.localScale;
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, target, t / time);
            yield return null;
        }

        transform.localScale = target;
    }
}
