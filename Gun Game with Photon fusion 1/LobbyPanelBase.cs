using UnityEngine;

public class LobbyPanelBase : MonoBehaviour
{
    public enum LobbyPanelType
    {
        None,
        CreateNicknamePanel,
        MiddleSectionPanel
    }

	[field: SerializeField, Header("LobbyPanelBase Vars")] // field: SerializeField�� �����Ϳ��� �����Ҽ� �ֵ�����
	public LobbyPanelType PanelType { get; private set; }

    [SerializeField] private Animator panelAnimator;
    protected LobbyUIManager lobbyUIManager; // protected�� �ؼ� LobbyPanelBase�� ��ӹ��� Ŭ�������� ����Ҽ��ֵ�����

	public virtual void InitPanel(LobbyUIManager uiManager)
    {
        lobbyUIManager = uiManager;
	}

    public void ShowPanel()
    {
        this.gameObject.SetActive(true);
        const string POP_IN_CLIP_NAME = "In";
        panelAnimator.Play(POP_IN_CLIP_NAME);
    }

    protected void ClosePanel()
    {
        const string POP_OUT_CLIP_NAME = "Out";
        StartCoroutine(Utils.PlayAnimAndSetStateWhenFinished(this.gameObject, panelAnimator, POP_OUT_CLIP_NAME, false));
    }
}
