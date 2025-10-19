using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryResultUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;

	private Animator animator;
	private const string POPUP = "Popup";

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
		DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
		this.gameObject.SetActive(false);
	}
	
	private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
	{
		this.gameObject.SetActive(true);
		backgroundImage.color = successColor;
		iconImage.sprite = successSprite;
		messageText.text = "DELIVERY\nSUCCESS";
		animator.SetTrigger(POPUP);
	}

	private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
	{
		this.gameObject.SetActive(true);
		backgroundImage.color = failedColor;
		iconImage.sprite = failedSprite;
		messageText.text = "DELIVERY\nFAILED";
		animator.SetTrigger(POPUP);
	}
}
