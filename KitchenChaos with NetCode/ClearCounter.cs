using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO; // 스크립터블로 만든거 에디터에서 할당

	public override void Interact(Player player) // 부모클래스 BaseCounter의 virtual함수 오버라이드
	{
		if (!HasKitchenObject()) // E눌렀을때 빈테이블위에는 오브젝트가없는데
		{
			if (player.HasKitchenObject()) // 플레이어가 오브젝트를 들고있으면
			{
				player.GetKitchenObject().SetKitchenObjectParent(this); // 빈테이블에 오브젝트를 놓는다
			}
			else // 플레이어가 오브젝트를 들고있지 않으면
			{
				
			}
		}
		else // 빈테이블위에 이미 오브젝트 있는데
		{
			if (player.HasKitchenObject()) // 플레이어가 오브젝트를 들고있는데
			{
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // 플레이어가 들고있는 오브젝트에 부착된 스크립트가 PlateKitchenObject이고(접시이고)
				{
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) // ClearCounter에 있는 오브젝트가 재료로 올라갈수 있는 오브젝트라면 접시에 넣고
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // ClearCounter에 있던 오브젝트 파괴
					}
				}
				else // 플레이어가 접시 이외의 것을 들고있을때
				{
					// 위에 out에서 선언된 plateKitchenObject 매개변수 사용
					if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) // ClearCounter에 있는 오브젝트가 접시이면
					{
						if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) // 플레이어가 들고있는 오브젝트가 접시에 넣을수 있는 재료면 넣고
						{
							KitchenObject.DestroyKitchenObject(player.GetKitchenObject()); // 플레이어가 들고있는 재료 파괴
						}
					}
				}
			}
			else // 플레이어가 오브젝트를 들고있지 않을때
			{
				GetKitchenObject().SetKitchenObjectParent(player); // 다시 플레이어가 오브젝트를 집는다
			}
		}
	}
}
