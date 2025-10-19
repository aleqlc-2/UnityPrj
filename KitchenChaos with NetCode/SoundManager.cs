using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance { get; private set; }

	[SerializeField] private AudioClipRefsSO audioClipRefsSO;

	private float volume = 1f;

	private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

	private void Awake()
	{
		Instance = this;
		volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f); // 두번째 인수는 얻어오지못했을때 디폴트값
	}

	private void Start()
	{
		TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
		BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
		Player.OnAnyPickedSomething += Instance_OnPickedSomething;
		CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut; ;
		DeliveryManager.Instance.OnRecipeSuccess += Instance_OnRecipeSuccess;
		DeliveryManager.Instance.OnRecipeFailed += Instance_OnRecipeFailed;
	}

	private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
	{
		TrashCounter trashCounter = sender as TrashCounter;
		PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
	}

	private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e)
	{
		BaseCounter baseCounter = sender as BaseCounter;
		PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
	}

	private void Instance_OnPickedSomething(object sender, System.EventArgs e)
	{
		Player player = sender as Player;
		PlaySound(audioClipRefsSO.objectPickup, player.transform.position);
	}

	private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
	{
		CuttingCounter cuttingCounter = sender as CuttingCounter;
		PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
	}

	private void Instance_OnRecipeSuccess(object sender, System.EventArgs e)
	{
		DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
		PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);
	}

	private void Instance_OnRecipeFailed(object sender, System.EventArgs e)
	{
		DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
		PlaySound(audioClipRefsSO.deliveryFail, deliveryCounter.transform.position);
	}

	public void PlayFootstepsSound(Vector3 position, float volume)
	{
		PlaySound(audioClipRefsSO.footstep, position, volume);
	}

	public void PlayCountdownSound()
	{
		PlaySound(audioClipRefsSO.warning, Vector3.zero);
	}

	public void PlayWarningSound(Vector3 position)
	{
		PlaySound(audioClipRefsSO.warning, position);
	}

	private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
		PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
	}

	private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
	{
		AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
	}

	public void ChangeVolume()
	{
		volume += 0.1f;
		if (volume > 1f)
		{
			volume = 0f;
		}

		PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
		PlayerPrefs.Save();
	}

	public float GetVolume()
	{
		return volume;
	}
}
