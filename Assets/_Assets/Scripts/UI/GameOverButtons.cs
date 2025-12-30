using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverButtons : MonoBehaviour
{
    private bool isClickLocked = false; 



    public void RestartGame(Transform button)
    {
        if (isClickLocked) return;
        isClickLocked = true;

        StartCoroutine(ButtonAnimation(button, () =>
        {
            PlayerPrefs.SetInt("SkipTutorial", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }));
    }


    public void OnQuitButton(Transform button)
    {
        if (isClickLocked) return;
        isClickLocked = true;

        StartCoroutine(ButtonAnimation(button, () =>
        {
            SceneManager.LoadScene(0);
        }));
    }


    private IEnumerator ButtonAnimation(Transform btn, System.Action callback)
    {
        Vector3 originalScale = btn.localScale;
        Vector3 enlargedScale = originalScale * 1.15f;

        float t = 0f;

    
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            btn.localScale = Vector3.Lerp(originalScale, enlargedScale, t);
            yield return null;
        }

       
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            btn.localScale = Vector3.Lerp(enlargedScale, originalScale, t);
            yield return null;
        }

        btn.localScale = originalScale;

      
        callback?.Invoke();
    }
}
