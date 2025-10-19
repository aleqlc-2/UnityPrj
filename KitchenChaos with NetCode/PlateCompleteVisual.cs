using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;

	[SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjectList;
	[Serializable] // class나 struct 에디터에 보여줌, 이 struct를 제너릭으로 삼은 kitchenObjectSOGameObjectList 리스트가 보임
	public struct KitchenObjectSO_GameObject // 참조타입인 class와 달리 struct는 value타입이라 값을 복사해서 보낸다
	{
		public KitchenObjectSO kitchenObjectSO; // 에디터에서 할당
		public GameObject gameObject; // 에디터에서 할당
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

	// 재료가 추가될때마다 완성된 음식의 부분 visual을 하나씩 활성화
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
