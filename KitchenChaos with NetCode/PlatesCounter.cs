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

    // player�� PlateCounter�տ��� E�� ��������
	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject()) // �÷��̾ ��� �ִ� ��ü�� ���µ�
        {
            if (platesSpawnedAmount > 0) // PlateCounter�� ���ð� �Ѱ��̻� �ִٸ�
			{
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player); // �÷��̾ ���ø� �Ѱ� ���
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
		OnPlateRemoved?.Invoke(this, EventArgs.Empty); // list���� �����ϰ� ���̺������ִ� ���� �Ѱ� �ı�
	}
}
