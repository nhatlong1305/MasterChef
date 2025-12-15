using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverButtons : MonoBehaviour
{
    private bool isClickLocked = false; // chống spam


    // ===========================
    //       RESTART GAME
    // ===========================
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


    // ===========================
    //         QUIT GAME
    // ===========================
    public void OnQuitButton(Transform button)
    {
        if (isClickLocked) return;
        isClickLocked = true;

        StartCoroutine(ButtonAnimation(button, () =>
        {
            SceneManager.LoadScene(0);
        }));
    }


    // ===========================
    //     BUTTON ANIMATION
    // ===========================
    private IEnumerator ButtonAnimation(Transform btn, System.Action callback)
    {
        Vector3 originalScale = btn.localScale;
        Vector3 enlargedScale = originalScale * 1.15f;

        float t = 0f;

        // Scale lên
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            btn.localScale = Vector3.Lerp(originalScale, enlargedScale, t);
            yield return null;
        }

        // Scale xuống
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            btn.localScale = Vector3.Lerp(enlargedScale, originalScale, t);
            yield return null;
        }

        // Giữ đúng kích thước gốc
        btn.localScale = originalScale;

        // Gọi function thực sự
        callback?.Invoke();
    }
}
