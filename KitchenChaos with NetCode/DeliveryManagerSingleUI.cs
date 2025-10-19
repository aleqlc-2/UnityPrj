using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour // Canvas -> RecipeTemplate 오브젝트에 부착된 스크립트
{
    [SerializeField] private TextMeshProUGUI recipeNameText; // UI하위계층에 있으므로 UGUI 붙여줘야함.
    [SerializeField] private Transform iconContainer; // Horizontal Layout Group
    [SerializeField] private Transform iconTemplate; // Image

	private void Awake()
	{
		iconTemplate.gameObject.SetActive(false);
	}

    // 좌상단 UI에 대기리스트 음식의 이름과 필요한재료 이미지 보여줌
	public void SetRecipeSO(RecipeSO recipeSO)
    {
		recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
		}
	}
}
