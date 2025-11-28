using UnityEngine;

public class PlateUIBillboard : MonoBehaviour
{
    public Transform target;      
    public Vector3 offset = new Vector3(0, 1f, 0);
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null) return;

        
        transform.position = target.position + offset;

       
        transform.LookAt(
            transform.position + cam.transform.forward,
            cam.transform.up
        );
    }
}
