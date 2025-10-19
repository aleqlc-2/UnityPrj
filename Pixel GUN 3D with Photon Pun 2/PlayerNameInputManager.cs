using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class PlayerNameInputManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;

    // �����Ϳ��� ��ǲ�ʵ� value changed�� ���ε�
    public void SetPlayerName()
    {
        if (string.IsNullOrEmpty(playerNameInputField.text))
        {
            Debug.Log("player name is empty");
            return;
        }

        PhotonNetwork.NickName = playerNameInputField.text;
    }
}
