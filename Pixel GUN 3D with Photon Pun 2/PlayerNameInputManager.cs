using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class PlayerNameInputManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;

    // 에디터에서 인풋필드 value changed에 바인딩
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
