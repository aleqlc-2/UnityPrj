using UnityEngine;

public class DeliveryCounter : BaseCounter
{
	public static DeliveryCounter Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public override void Interact(Player player)
	{
		if (player.HasKitchenObject())
		{
			if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // �÷��̾ ����ִ°� ���ø�
			{
				DeliveryManager.Instance.DeliverRecipe(plateKitchenObject); // �÷��̾ ������ ���ÿ� �ִ� ���� ����� �ִ� ������ ��⿭�� �ִ��� üũ

				KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
			}
		}
	}
}
