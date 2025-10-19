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
			if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // 플레이어가 들고있는게 접시면
			{
				DeliveryManager.Instance.DeliverRecipe(plateKitchenObject); // 플레이어가 가져온 접시에 있는 재료로 만들수 있는 음식이 대기열에 있는지 체크

				KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
			}
		}
	}
}
