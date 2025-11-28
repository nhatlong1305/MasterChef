using System;
using UnityEngine;

public class TrashCounter : BaseCounter
{

    public static event EventHandler OnAnyobjectTrashed;
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            player.GetKitchenObject().DestroySelf();

            OnAnyobjectTrashed?.Invoke(this,EventArgs.Empty);
        }
    }
}
