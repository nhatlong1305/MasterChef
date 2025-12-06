using AllIn13DShader;
using UnityEngine;

public class OutlineAnimatorController : MonoBehaviour
{
    [SerializeField] private AllIn13DShaderShaderPropertyCurveAnim outlineAnim;

    private void Awake()
    {
        if (outlineAnim == null)
            outlineAnim = GetComponent<AllIn13DShaderShaderPropertyCurveAnim>();

        DisableOutline();
    }

    public void EnableOutline()
    {
        if (outlineAnim == null) return;

        outlineAnim.enabled = true;   
        outlineAnim.maxValue = 5f;    


    }

    public void DisableOutline()
    {
        if (outlineAnim == null) return;

        outlineAnim.maxValue = 0f;    


    }
}
