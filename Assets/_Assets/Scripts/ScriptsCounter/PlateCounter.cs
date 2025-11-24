using System;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;


    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;


    private float spawnPlateTimer;
    private float spawnPlateTimerMax =4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;
    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax) {

            spawnPlateTimer = 0f;
            if (platesSpawnedAmount < platesSpawnedAmountMax) {
                platesSpawnedAmount ++;
                OnPlateSpawned?.Invoke(this,EventArgs.Empty);
            }
        }
    }
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount > 0)
            {
                platesSpawnedAmount--;

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

                OnPlateRemoved?.Invoke(this,EventArgs.Empty);
            }
        }
    }

}
