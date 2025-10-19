using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
	[SerializeField] private BaseCounter baseCounter;
	[SerializeField] private GameObject[] visualGameObjectArray; // 흰색반투명 오브젝트

	private void Start()
	{
		//if (Player.LocalInstance != null) // 플레이어마다 Player.LocalInstance 다름 두번째부터 생성된 플레이어도 if로 들어올수없음
		//{
		//	Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		//}
		//else // SelectedCounterVisual의 Start는 호출되었고 첫 플레이어(Host)가 생성되기 전
		//{
		//	Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
		//}

		Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
	}

	private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
	{
		if (e.selectedCounter == baseCounter) // e.selectedCounter는 플레이어가 레이로 맞춘 오브젝트의 baseCounter == 대상이되는 오브젝트 자기자신의 baseCounter
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
	{
		if (Player.LocalInstance != null)
		{
			//Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged; // 플레이어마다 Player.LocalInstance 다름 빼줄필요없음
			Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		}
	}
	
	private void Show()
	{
		foreach (GameObject visualGameObject in visualGameObjectArray)
		{
			visualGameObject.SetActive(true);
		}		
	}

	private void Hide()
	{
		foreach (GameObject visualGameObject in visualGameObjectArray)
		{
			visualGameObject.SetActive(false);
		}
	}
}
