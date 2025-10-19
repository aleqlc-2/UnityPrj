using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO; // ��ũ���ͺ�� ����� �����Ϳ��� �Ҵ�

	public override void Interact(Player player) // �θ�Ŭ���� BaseCounter�� virtual�Լ� �������̵�
	{
		if (!HasKitchenObject()) // E�������� �����̺������� ������Ʈ�����µ�
		{
			if (player.HasKitchenObject()) // �÷��̾ ������Ʈ�� ���������
			{
				player.GetKitchenObject().SetKitchenObjectParent(this); // �����̺� ������Ʈ�� ���´�
			}
			else // �÷��̾ ������Ʈ�� ������� ������
			{
				
			}
		}
		else // �����̺����� �̹� ������Ʈ �ִµ�
		{
			if (player.HasKitchenObject()) // �÷��̾ ������Ʈ�� ����ִµ�
			{
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // �÷��̾ ����ִ� ������Ʈ�� ������ ��ũ��Ʈ�� PlateKitchenObject�̰�(�����̰�)
				{
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) // ClearCounter�� �ִ� ������Ʈ�� ���� �ö󰥼� �ִ� ������Ʈ��� ���ÿ� �ְ�
					{
						KitchenObject.DestroyKitchenObject(GetKitchenObject()); // ClearCounter�� �ִ� ������Ʈ �ı�
					}
				}
				else // �÷��̾ ���� �̿��� ���� ���������
				{
					// ���� out���� ����� plateKitchenObject �Ű����� ���
					if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) // ClearCounter�� �ִ� ������Ʈ�� �����̸�
					{
						if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) // �÷��̾ ����ִ� ������Ʈ�� ���ÿ� ������ �ִ� ���� �ְ�
						{
							KitchenObject.DestroyKitchenObject(player.GetKitchenObject()); // �÷��̾ ����ִ� ��� �ı�
						}
					}
				}
			}
			else // �÷��̾ ������Ʈ�� ������� ������
			{
				GetKitchenObject().SetKitchenObjectParent(player); // �ٽ� �÷��̾ ������Ʈ�� ���´�
			}
		}
	}
}
