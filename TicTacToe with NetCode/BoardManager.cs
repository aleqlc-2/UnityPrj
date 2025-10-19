using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class BoardManager : NetworkBehaviour
{
    private Button[,] buttons = new Button[3, 3];

	[SerializeField] private Sprite xSprite, oSprite;

	// Board�� �����Ǹ� Board�������� ��ư�鿡 �ִ� Button������Ʈ�� �迭 ���������� ��´�
	public override void OnNetworkSpawn()
	{
		var cells = GetComponentsInChildren<Button>();
		int n = 0;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				buttons[i, j] = cells[n];
				n++;

				int r = i;
				int c = j;

				buttons[i, j].onClick.AddListener(delegate
				{
					OnClickCell(r, c);
				});
			}
		}
	}

	// 1:1����. ȣ��Ʈ�� ������ Xǥ��, Ŭ���̾�Ʈ�� ������ Oǥ��
	private void OnClickCell(int r, int c)
	{
		if (NetworkManager.Singleton.IsHost && GameManager.Instance.currentTurn.Value == 0)
		{
			buttons[r, c].GetComponent<Image>().sprite = xSprite;
			buttons[r, c].interactable = false;
			ChangeSpriteClientRpc(r, c); // ȣ��Ʈ�� sprite�� �������� Ŭ���̾�Ʈ���Ե� �˷���
			CheckResult(r, c);
			GameManager.Instance.currentTurn.Value = 1;
		}
		else if (!NetworkManager.Singleton.IsHost && GameManager.Instance.currentTurn.Value == 1)
		{
			buttons[r, c].GetComponent<Image>().sprite = oSprite;
			buttons[r, c].interactable = false;
			ChangeSpriteServerRpc(r, c); // Ŭ���̾�Ʈ�� sprite�� �������� ȣ��Ʈ���Ե� �˷���
			CheckResult(r, c);
		}
	}

	// ȣ��Ʈ Ŭ�� �Ѵ� �ڵ����
	[ClientRpc]
	private void ChangeSpriteClientRpc(int r, int c)
	{
		buttons[r, c].GetComponent<Image>().sprite = xSprite;
		buttons[r, c].interactable = false;
	}

	// (RequireOwnership = false) �������� Ŭ���ʿ��� ������
	// ServerRpc�� Ownership�� �����ʸ� ȣ�Ⱑ���ѵ� Ŭ��� Ownership�� �����Ƿ� RequireOwnership = false�� ��� Ŭ��� �ڵ带 ���������ʵ����Ѵ�.
	// ȣ��Ʈ�� �ڵ� ����
	[ServerRpc(RequireOwnership = false)] 
	private void ChangeSpriteServerRpc(int r, int c)
	{
		buttons[r, c].GetComponent<Image>().sprite = oSprite;
		buttons[r, c].interactable = false;
		GameManager.Instance.currentTurn.Value = 0;
	}

	public bool IsWon(int r, int c)
	{
		Sprite clickedButtonSprite = buttons[r, c].GetComponent<Image>().sprite;

		// �� ���� ������ �¸�
		if (buttons[0, c].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
			buttons[1, c].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
			buttons[2, c].GetComponentInChildren<Image>().sprite == clickedButtonSprite)
		{
			return true;
		}
		// �Ǵ� �� ���� ������ �¸�
		else if (buttons[r, 0].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[r, 1].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[r, 2].GetComponentInChildren<Image>().sprite == clickedButtonSprite)
		{
			return true;
		}
		// �Ǵ� ������ �밢���� ������ �¸�
		else if (buttons[0, 0].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[1, 1].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[2, 2].GetComponentInChildren<Image>().sprite == clickedButtonSprite)
		{
			return true;
		}
		// �Ǵ� ����� �밢���� ������ �¸�
		else if (buttons[0, 2].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[1, 1].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[2, 0].GetComponentInChildren<Image>().sprite == clickedButtonSprite)
		{
			return true;
		}

		return false;
	}

	private bool IsGameDraw()
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				// ���� ����ִ� ���� �ִٸ� draw�ƴ�
				if (buttons[i, j].GetComponent<Image>().sprite != xSprite &&
					buttons[i, j].GetComponent<Image>().sprite != oSprite)
				{
					return false;
				}
			}
		}

		return true;
	}

	private void CheckResult(int r, int c)
	{
		if (IsWon(r, c))
		{
			GameManager.Instance.ShowMsg("won");
		}
		else
		{
			if (IsGameDraw())
			{
				GameManager.Instance.ShowMsg("draw");
			}
		}
	}
}
