using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;

	// �������̽��ڷ����� �����Ϳ��� �Ҵ��� �� �����Ƿ� IHasProgress�������̽��� ����ϴ� Ŭ������ ��ũ��Ʈ�� ������ GameObject�� �޾Ƽ� ������Ʈ�� ã�½�����
	[SerializeField] private GameObject hasProgressGameObject;

	private IHasProgress hasProgress; // �������̽��� �̺�Ʈ�����Ͽ� �������� progress�� �ٷ絵��

	private void Start()
	{
		hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
		if (hasProgress == null)
		{
			Debug.LogError(hasProgressGameObject + "�� IHasProgress�� ������ ������Ʈ�� �����������ʽ��ϴ�.");
		}
		hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;

		barImage.fillAmount = 0f;
		Hide();
	}

	private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
	{
		barImage.fillAmount = e.progressNormalized;

		if (e.progressNormalized == 0f || e.progressNormalized == 1f)
			Hide();
		else
			Show();
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}
}
