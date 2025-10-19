using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour // Canvas -> RecipeTemplate ������Ʈ�� ������ ��ũ��Ʈ
{
    [SerializeField] private TextMeshProUGUI recipeNameText; // UI���������� �����Ƿ� UGUI �ٿ������.
    [SerializeField] private Transform iconContainer; // Horizontal Layout Group
    [SerializeField] private Transform iconTemplate; // Image

	private void Awake()
	{
		iconTemplate.gameObject.SetActive(false);
	}

    // �»�� UI�� ��⸮��Ʈ ������ �̸��� �ʿ������ �̹��� ������
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
