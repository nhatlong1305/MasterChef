using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private Vector3 normalScale = Vector3.one;
    private Vector3 hoverScale = Vector3.one * 1.1f;
    private Vector3 clickScale = Vector3.one * 0.9f;

    private void Awake()
    {
        AddEffects(playButton, () =>
        {
            Loader.Load(Loader.Scene.Kitchen);
        });

        AddEffects(quitButton, () =>
        {
            Application.Quit();
        });
    }
    public void OnPlayButton()
    {
        SoundManager.Instance.PlayUIClickSound();
        Loader.Load(Loader.Scene.Kitchen);
    }

    public void OnQuitButton()
    {
        SoundManager.Instance.PlayUIClickSound();
        Application.Quit();
    }

    private void AddEffects(Button btn, System.Action action)
    {
        EventTriggerListener listener = btn.gameObject.AddComponent<EventTriggerListener>();

        listener.onEnter = () => StartCoroutine(ScaleTo(btn.transform, hoverScale, 0.12f));
        listener.onExit = () => StartCoroutine(ScaleTo(btn.transform, normalScale, 0.12f));

        listener.onClick = () =>
        {
            StartCoroutine(ButtonClickAnimation(btn.transform, action));
        };
    }

    private IEnumerator ButtonClickAnimation(Transform t, System.Action action)
    {
        yield return StartCoroutine(ScaleTo(t, clickScale, 0.08f));

      
        yield return StartCoroutine(ScaleTo(t, normalScale, 0.12f));

        action?.Invoke();
    }

    private IEnumerator ScaleTo(Transform target, Vector3 newScale, float time)
    {
        Vector3 start = target.localScale;
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(start, newScale, t / time);
            yield return null;
        }

        target.localScale = newScale;
    }
}
