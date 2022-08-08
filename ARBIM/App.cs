using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Android;

public class App : MonoBehaviour
{
	public static App instance;

	//// 디버깅텍스트
	[SerializeField] private TextMeshProUGUI TouchStartText;
	//[SerializeField] private TextMeshProUGUI CaptureText;
	//[SerializeField] private TextMeshProUGUI AddImageStartText;
	//[SerializeField] private TextMeshProUGUI AddImageEndText;
	//[SerializeField] private TextMeshProUGUI ImageStartChangeText;
	//[SerializeField] private TextMeshProUGUI ImageAddChangeText;
	//[SerializeField] private TextMeshProUGUI ImageUpdateChangeText;
	//[SerializeField] private TextMeshProUGUI ImageRemoveChangeText;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void Start()
    {
		//      // 유니티 프로젝트 폴더안의 에셋번들을 instantiate
		//      StartCoroutine(AssetManager.instance.LoadFromMemoryAsync("Assets/AssetBundles/building", (bundle) =>
		//{
		//          Debug.LogFormat("bundle: {0}", bundle);
		//          var prefab = bundle.LoadAsset<GameObject>("202bundle"); // 번들안에 있는 특정한 에셋을 로드
		//          var model = Instantiate<GameObject>(prefab);
		//}));

		// // 서버에 있는 에셋번들 바이너리화 없이 바로 가져옴
		//StartCoroutine(AssetManager.instance.LoadFromServer("C:\\Users\\TAESUNG SNI\\Desktop\\NodeToUnity\\AssetBundles\\building"));

		//// 서버에 있는 에셋번들 바이너리화하여 가져와서 로컬저장소에 write, PC
		//StartCoroutine(AssetManager.instance.LoadFromServer("C:\\Users\\TAESUNG SNI\\Desktop\\NodeToUnity\\AssetBundles", "building"));

		// 서버에 있는 에셋번들 바이너리화하여 가져와서 로컬저장소에 write
		if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
		{
			Permission.RequestUserPermission(Permission.ExternalStorageWrite);
		}
		StartCoroutine(AssetManager.instance.LoadFromServer("C:\\Users\\TAESUNG SNI\\Desktop\\NodeToUnity\\AssetBundles", "building"));
	}

    public void LoadAsset()
	{
		//// 서버에 있는 에셋번들 비동기로 바이너리화하여 가져옴, 딕셔너리에서 빼옴, PC
		//StartCoroutine(AssetManager.instance.LoadFromFileAsync(Application.persistentDataPath, "building", () =>
		//{
		//	  Debug.Log(Application.persistentDataPath.ToString());
		//    var prefab = AssetManager.instance.LoadAsset("building", "202bundle");
		//    var go = Instantiate<GameObject>(prefab);
		//}));

		// 서버에 있는 에셋번들 비동기로 바이너리화하여 가져옴, 딕셔너리에서 빼옴, AR Mobile
		StartCoroutine(AssetManager.instance.LoadFromFileAsync(Application.persistentDataPath, "building", () =>
		{
			TouchStartText.text = Application.persistentDataPath.ToString();
			var prefab = AssetManager.instance.LoadAsset("building", "202bundle");
			TouchManager.instance.PlaceObject = prefab;
		}));
	}
}
