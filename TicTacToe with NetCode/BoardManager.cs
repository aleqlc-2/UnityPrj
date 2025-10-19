using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class BoardManager : NetworkBehaviour
{
    private Button[,] buttons = new Button[3, 3];

	[SerializeField] private Sprite xSprite, oSprite;

	// Board가 생성되면 Board하위계층 버튼들에 있는 Button컴포넌트를 배열 지역변수에 담는다
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

	// 1:1게임. 호스트가 누르면 X표시, 클라이언트가 누르면 O표시
	private void OnClickCell(int r, int c)
	{
		if (NetworkManager.Singleton.IsHost && GameManager.Instance.currentTurn.Value == 0)
		{
			buttons[r, c].GetComponent<Image>().sprite = xSprite;
			buttons[r, c].interactable = false;
			ChangeSpriteClientRpc(r, c); // 호스트의 sprite가 변했음을 클라이언트에게도 알려줌
			CheckResult(r, c);
			GameManager.Instance.currentTurn.Value = 1;
		}
		else if (!NetworkManager.Singleton.IsHost && GameManager.Instance.currentTurn.Value == 1)
		{
			buttons[r, c].GetComponent<Image>().sprite = oSprite;
			buttons[r, c].interactable = false;
			ChangeSpriteServerRpc(r, c); // 클라이언트의 sprite가 변했음을 호스트에게도 알려줌
			CheckResult(r, c);
		}
	}

	// 호스트 클라 둘다 코드실행
	[ClientRpc]
	private void ChangeSpriteClientRpc(int r, int c)
	{
		buttons[r, c].GetComponent<Image>().sprite = xSprite;
		buttons[r, c].interactable = false;
	}

	// (RequireOwnership = false) 안적으면 클라쪽에서 에러뜸
	// ServerRpc는 Ownership을 가진쪽만 호출가능한데 클라는 Ownership이 없으므로 RequireOwnership = false를 적어서 클라는 코드를 실행하지않도록한다.
	// 호스트만 코드 실행
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

		// 한 열이 같으면 승리
		if (buttons[0, c].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
			buttons[1, c].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
			buttons[2, c].GetComponentInChildren<Image>().sprite == clickedButtonSprite)
		{
			return true;
		}
		// 또는 한 행이 같으면 승리
		else if (buttons[r, 0].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[r, 1].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[r, 2].GetComponentInChildren<Image>().sprite == clickedButtonSprite)
		{
			return true;
		}
		// 또는 좌하향 대각선이 같으면 승리
		else if (buttons[0, 0].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[1, 1].GetComponentInChildren<Image>().sprite == clickedButtonSprite &&
				 buttons[2, 2].GetComponentInChildren<Image>().sprite == clickedButtonSprite)
		{
			return true;
		}
		// 또는 우상향 대각선이 같으면 승리
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
				// 아직 비어있는 셀이 있다면 draw아님
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
