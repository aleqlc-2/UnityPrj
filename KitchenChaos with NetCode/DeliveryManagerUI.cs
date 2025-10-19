using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

	private void Awake()
	{
		recipeTemplate.gameObject.SetActive(false);
	}

	private void Start()
	{
		DeliveryManager.Instance.OnRecipeSpawned += Delivery_OnRecipeSpawned;
		DeliveryManager.Instance.OnRecipeCompleted += Delivery_OnRecipeCompleted;
		UpdateVisual();
	}

	private void Delivery_OnRecipeSpawned(object sender, System.EventArgs e)
	{
		UpdateVisual();
	}

	private void Delivery_OnRecipeCompleted(object sender, System.EventArgs e)
	{
		UpdateVisual();
	}
	
	private void UpdateVisual()
	{
		foreach (Transform child in container)
		{
			if (child == recipeTemplate) continue; // recipeTemplate는 파괴하지않는다

			Destroy(child.gameObject);
		}

		foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
		{
			Transform recipeTransform = Instantiate(recipeTemplate, container); // 비활성화한 오브젝트를 Instantiate하면 비활성화된채로 만들어짐
			recipeTransform.gameObject.SetActive(true);
			recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
		}
	}
}
