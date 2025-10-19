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
		// 재료가 추가될때 이 스크립트가 달린 오브젝트의 자식개체중 transform이 iconTemplate가 아닌 오브젝트 전부 파괴
		// 미리 넣어놓고 비활성화해놓은 iconTemplate를 Instantiate하는 것이므로 생성되는 모든 iconTemplate의 transform은 iconTemplate임
		foreach (Transform child in transform)
		{
			if (child == iconTemplate) continue;
			Destroy(child.gameObject);
		}

		foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
		{
			Transform iconTransform = Instantiate(iconTemplate, transform); // 생성된 iconTemplate오브젝트의 부모는 이 스트립트가 달린 오브젝트
			iconTransform.gameObject.SetActive(true);
			iconTransform.GetComponent<PlateIconsSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
		}
	}
}
