using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class PlatesCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    public event EventHandler OnPlateSpawned;
	public event EventHandler OnPlateRemoved;

	private void Update()
    {
        if (!IsServer) return;

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
            {
                SpawnPlateServerRpc();
			}
        }
    }

    [ServerRpc] // if (!IsServer) return;
	private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
	}

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
		platesSpawnedAmount++;
		OnPlateSpawned?.Invoke(this, EventArgs.Empty);
	}

    // player가 PlateCounter앞에서 E를 눌렀을때
	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject()) // 플레이어가 들고 있는 물체가 없는데
        {
            if (platesSpawnedAmount > 0) // PlateCounter에 접시가 한개이상 있다면
			{
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player); // 플레이어가 접시를 한개 든다
                InteractLogicServerRpc();
			}
        }
	}

	[ServerRpc(RequireOwnership = false)]
	private void InteractLogicServerRpc()
	{
		InteractLogicClientRpc();
	}

	[ClientRpc]
	private void InteractLogicClientRpc()
	{
		platesSpawnedAmount--;
		OnPlateRemoved?.Invoke(this, EventArgs.Empty); // list에서 삭제하고 테이블위에있는 접시 한개 파괴
	}
}
