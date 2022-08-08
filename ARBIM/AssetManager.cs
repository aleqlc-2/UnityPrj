using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;

public class AssetManager : MonoBehaviour
{
    public static AssetManager instance;
    public Dictionary<string, AssetBundle> dicBundles = new Dictionary<string, AssetBundle>();

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

 //   // 유니티 내부의 에셋번들 파일을 바이트배열로 읽어서 비동기 방식으로 로드
 //   public IEnumerator LoadFromMemoryAsync(string path, System.Action<AssetBundle> callback)
	//{
 //       byte[] binary = File.ReadAllBytes(path);
 //       Debug.Log("binary.Length : " + binary.Length);
 //       AssetBundleCreateRequest req = AssetBundle.LoadFromMemoryAsync(binary);
 //       yield return req;
 //       callback(req.assetBundle);
	//}

    // 서버에서 읽어와서 로컬저장소에 write해놓은 에셋번들을 딕셔너리에 저장하고 콜백을 호출하여 instantiate
    public IEnumerator LoadFromFileAsync(string path, string fileName, System.Action callback)
    {
        var req = AssetBundle.LoadFromFileAsync(string.Format("{0}/{1}", path, fileName));
        yield return req;
        var bundle = req.assetBundle;
        Debug.LogFormat("bundle: {0}", bundle);
        dicBundles.Add(fileName, bundle);
        callback();
    }

    // 서버에서 가져온 번들에서 특정 에셋 로드
    public GameObject LoadAsset(string bundleName, string prefabName)
    {
        return this.dicBundles[bundleName].LoadAsset<GameObject>(prefabName);
    }

 //   // 서버폴더에 있는 에셋번들 읽어옴. 추가동작은 없음
 //   public IEnumerator LoadFromServer(string uri)
	//{
	//	UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(uri);
	//	yield return req.SendWebRequest();

	//	AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
	//	Debug.Log(bundle);
	//}

    // 서버에서 에셋번들 가져와서 로컬저장소에 write
	public IEnumerator LoadFromServer(string path, string fileName)
    {
        // 서버에서 building 바이트로 가져옴
        string bundleUri = string.Format("{0}\\{1}", path, fileName);
        Debug.Log(bundleUri);
        UnityWebRequest req1 = UnityWebRequest.Get(bundleUri);
        yield return req1.SendWebRequest();
        var bytes = req1.downloadHandler.data;
        Debug.Log("bytes.Length : " + bytes.Length);

        // 서버에서 building.manifest 를 문자열로 가져옴
        var manifestUri = string.Format("{0}.manifest", bundleUri);
        UnityWebRequest req2 = UnityWebRequest.Get(manifestUri);
        yield return req2.SendWebRequest();
        var manifest = Encoding.UTF8.GetString(req2.downloadHandler.data);

        // building을 로컬저장소에 write
        string bundlePath = string.Format("{0}\\{1}", Application.persistentDataPath, fileName);
        Debug.Log("bundlePath : " + bundlePath);
        File.WriteAllBytes(bundlePath, bytes);

        // building.manifest를 로컬저장소에 write
        string manifestPath = string.Format("{0}.manifest", bundlePath);
        File.WriteAllBytes(manifestPath, ObjectToByteArray(manifest));
    }

    // building.manifest를 배열로 바꿔서 반환
    private byte[] ObjectToByteArray(object obj)
	{
        if (obj == null) return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
	}
}
