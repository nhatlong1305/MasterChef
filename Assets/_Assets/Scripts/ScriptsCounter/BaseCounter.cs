using System;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlaceHere;

    [SerializeField] protected Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public virtual void Interact(Player player)
    {
        // override ở các counter con
    }

    public virtual void InteractAlternate(Player player)
    {
        // override nếu counter cần (Cutting, Stove ...)
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnAnyObjectPlaceHere?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    // ================================
    //      Hỗ trợ Tutorial
    // ================================

    public bool HasPlateOnCounter()
    {
        if (!HasKitchenObject()) return false;
        return GetKitchenObject() is PlateKitchenObject;
    }

    public bool HasIngredient(KitchenObjectSO ingredient)
    {
        if (!HasKitchenObject()) return false;
        return GetKitchenObject().GetKitchenObjectSO() == ingredient;
    }

    public bool IsPlayerNear(float distance = 1.5f)
    {
        if (Player.Instance == null) return false;

        float d = Vector3.Distance(Player.Instance.transform.position, transform.position);
        return d <= distance;
    }
}
