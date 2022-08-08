using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles : MonoBehaviour
{
    [MenuItem("Assets/Build AssetBundles")]
    private static void BuildAllAssetBundles()
	{
		string path = "Assets/AssetBundles";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		
		BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.Android);
	}
}
