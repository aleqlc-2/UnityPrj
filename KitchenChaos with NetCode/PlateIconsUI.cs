using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
	[SerializeField] private Transform iconTemplate;

	private void Awake()
	{
		iconTemplate.gameObject.SetActive(false);
	}

	private void Start()
	{
		plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
	}

	private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
	{
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		// ��ᰡ �߰��ɶ� �� ��ũ��Ʈ�� �޸� ������Ʈ�� �ڽİ�ü�� transform�� iconTemplate�� �ƴ� ������Ʈ ���� �ı�
		// �̸� �־���� ��Ȱ��ȭ�س��� iconTemplate�� Instantiate�ϴ� ���̹Ƿ� �����Ǵ� ��� iconTemplate�� transform�� iconTemplate��
		foreach (Transform child in transform)
		{
			if (child == iconTemplate) continue;
			Destroy(child.gameObject);
		}

		foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
		{
			Transform iconTransform = Instantiate(iconTemplate, transform); // ������ iconTemplate������Ʈ�� �θ�� �� ��Ʈ��Ʈ�� �޸� ������Ʈ
			iconTransform.gameObject.SetActive(true);
			iconTransform.GetComponent<PlateIconsSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
		}
	}
}
