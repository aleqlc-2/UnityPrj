using Fusion;
using TMPro;
using UnityEngine;

public class PlayerChatController : NetworkBehaviour
{
	[Networked] public bool IsTyping { get; private set; }

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Animator bubbleAnimator;
    [SerializeField] private TextMeshProUGUI bubbleText;

	public override void Spawned()
	{
		var isLocalPlayer = Object.InputAuthority == Runner.LocalPlayer;
		gameObject.SetActive(isLocalPlayer);

		if (isLocalPlayer)
		{
			inputField.onSelect.AddListener((string arg0) => Rpc_UpdateServerTypingStatus(true)); // inputField에 포커싱됐을때
			inputField.onDeselect.AddListener((string arg0) => Rpc_UpdateServerTypingStatus(false)); // inputField에 언포커싱됐을때

			// inputField.onSubmit은 편집끝내고 엔터눌렀을때 호출되는 이벤트
			inputField.onSubmit.AddListener(OnInputFieldSubmit);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private void Rpc_UpdateServerTypingStatus(bool isTyping)
	{
		IsTyping = isTyping;
	}

	private void OnInputFieldSubmit(string arg0)
	{
		if (!string.IsNullOrEmpty(arg0))
		{
			RpcSetBubbleSpeech(arg0);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	private void RpcSetBubbleSpeech(NetworkString<_64> txt)
	{
		bubbleText.text = txt.Value;
		const string TRIGGER = "Open";
		bubbleAnimator.SetTrigger(TRIGGER);
	}
}
