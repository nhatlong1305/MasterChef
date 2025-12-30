using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button == null) return;
        if (!button.interactable) return;
        if (SoundManager.Instance == null) return;
       

        SoundManager.Instance.PlayUIHoverSound();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button == null) return;
        if (!button.interactable) return;
        if (SoundManager.Instance == null) return;
      

        SoundManager.Instance.PlayUIClickSound();
    }
}
