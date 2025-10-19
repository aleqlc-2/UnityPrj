using UnityEngine;

public class StoveBurnWarningUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

	private void Start()
	{
		stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
		Hide();
	}

	private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
	{
		float burnShowProgressAmount = 0.5f;
		bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;

		if (show)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Show()
	{
		this.gameObject.SetActive(true);
	}

	private void Hide()
	{
		this.gameObject.SetActive(false);
	}
}
