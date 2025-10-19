using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;

	[SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjectList;
	[Serializable] // class�� struct �����Ϳ� ������, �� struct�� ���ʸ����� ���� kitchenObjectSOGameObjectList ����Ʈ�� ����
	public struct KitchenObjectSO_GameObject // ����Ÿ���� class�� �޸� struct�� valueŸ���̶� ���� �����ؼ� ������
	{
		public KitchenObjectSO kitchenObjectSO; // �����Ϳ��� �Ҵ�
		public GameObject gameObject; // �����Ϳ��� �Ҵ�
	}

	private void Start()
	{
		Debug.Log(this.gameObject.name);

		plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

		foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
		{
			kitchenObjectSOGameObject.gameObject.SetActive(false);
		}
	}

	// ��ᰡ �߰��ɶ����� �ϼ��� ������ �κ� visual�� �ϳ��� Ȱ��ȭ
	private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
	{
		foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
		{
			if (kitchenObjectSOGameObject.kitchenObjectSO == e.kitchenObjectSO)
			{
				kitchenObjectSOGameObject.gameObject.SetActive(true);
			}
		}
	}
}
