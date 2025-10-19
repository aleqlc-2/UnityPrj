using UnityEngine;
using TMPro;

public class NamePlate : MonoBehaviour
{
	public TextMeshPro nameText;

	public void SetNickName(string nickName)
	{
		nameText.text = nickName;
	}

	private void LateUpdate()
	{
		// 캐릭터가 카메라 바라보는 방향으로 nameText바라보도록
		nameText.transform.rotation = Camera.main.transform.rotation;
	}
}
