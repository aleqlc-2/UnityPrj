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
		// ĳ���Ͱ� ī�޶� �ٶ󺸴� �������� nameText�ٶ󺸵���
		nameText.transform.rotation = Camera.main.transform.rotation;
	}
}
