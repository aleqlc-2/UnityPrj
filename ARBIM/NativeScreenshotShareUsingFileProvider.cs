using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NativeScreenshotShareUsingFileProvider : MonoBehaviour
{
	public void PickupPicture()
	{
#if UNITY_ANDROID
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_GET_CONTENT"));
		intentObject.Call<AndroidJavaObject>("setType", "image/jpg");
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		unityActivity.Call("startActivity", intentObject);
#endif
	}
}