using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;

	// 인터페이스자료형은 에디터에서 할당할 수 없으므로 IHasProgress인터페이스를 상속하는 클래스의 스크립트가 부착된 GameObject로 받아서 컴포넌트를 찾는식으로
	[SerializeField] private GameObject hasProgressGameObject;

	private IHasProgress hasProgress; // 인터페이스에 이벤트선언하여 여러가지 progress를 다루도록

	private void Start()
	{
		hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
		if (hasProgress == null)
		{
			Debug.LogError(hasProgressGameObject + "가 IHasProgress를 실행할 컴포넌트를 가지고있지않습니다.");
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
