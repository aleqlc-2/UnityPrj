using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
	[SerializeField] private BaseCounter baseCounter;
	[SerializeField] private GameObject[] visualGameObjectArray; // ��������� ������Ʈ

	private void Start()
	{
		//if (Player.LocalInstance != null) // �÷��̾�� Player.LocalInstance �ٸ� �ι�°���� ������ �÷��̾ if�� ���ü�����
		//{
		//	Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		//}
		//else // SelectedCounterVisual�� Start�� ȣ��Ǿ��� ù �÷��̾�(Host)�� �����Ǳ� ��
		//{
		//	Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
		//}

		Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
	}

	private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
	{
		if (e.selectedCounter == baseCounter) // e.selectedCounter�� �÷��̾ ���̷� ���� ������Ʈ�� baseCounter == ����̵Ǵ� ������Ʈ �ڱ��ڽ��� baseCounter
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
			//Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged; // �÷��̾�� Player.LocalInstance �ٸ� �����ʿ����
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
