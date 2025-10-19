using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class MiddleSectionPanel : LobbyPanelBase
{
    [Header("MiddleSectionPanel Vars")]
    [SerializeField] private Button joinRandomRoomBtn;
    [SerializeField] private Button joinRoomByArgBtn;
    [SerializeField] private Button createRoomBtn;

    [SerializeField] private TMP_InputField joinRoomByArgInputField;
    [SerializeField] private TMP_InputField createRoomInputField;

    private NetworkRunnerController networkRunnerController;

	public override void InitPanel(LobbyUIManager uiManager)
	{
		base.InitPanel(uiManager);

        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;
		createRoomBtn.onClick.AddListener(() => CreateRoom(GameMode.Host, createRoomInputField.text));
		joinRoomByArgBtn.onClick.AddListener(() => CreateRoom(GameMode.Client, joinRoomByArgInputField.text));
		joinRandomRoomBtn.onClick.AddListener(JoinRandomRoom);
	}

	private void CreateRoom(GameMode mode, string field)
    {
        if (field.Length >= 2)
        {
            Debug.Log("join in " + mode);
			networkRunnerController.StartGame(mode, field);
        }
    }

    private void JoinRandomRoom()
    {
        Debug.Log("join in RandomRoom");
		networkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
	}
}
