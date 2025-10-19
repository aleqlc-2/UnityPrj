using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// 에디터에서 플레이모드로 진입시 0번 scene이 아닌경우 0번 scene으로 이동시키는 스크립트
[InitializeOnLoad] // 정적 생성자가 호출되므로, 정적 생성자에 원하는 코드를 작성한다. Awake() 호출 이전 실행(오브젝트에 붙이지 않고 작성만해두면 실행됨)
public static class StartupSceneLoader
{
	// 정적 생성자. 한번만 자동호출
	static StartupSceneLoader()
	{
		EditorApplication.playModeStateChanged += LoadStartupScene;
	}

	private static void LoadStartupScene(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.ExitingEditMode)
		{
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		}

		if (state == PlayModeStateChange.EnteredPlayMode)
		{
			if (EditorSceneManager.GetActiveScene().buildIndex != 0)
			{
				EditorSceneManager.LoadScene(0);
			}
		}
	}
}

