using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// �����Ϳ��� �÷��̸��� ���Խ� 0�� scene�� �ƴѰ�� 0�� scene���� �̵���Ű�� ��ũ��Ʈ
[InitializeOnLoad] // ���� �����ڰ� ȣ��ǹǷ�, ���� �����ڿ� ���ϴ� �ڵ带 �ۼ��Ѵ�. Awake() ȣ�� ���� ����(������Ʈ�� ������ �ʰ� �ۼ����صθ� �����)
public static class StartupSceneLoader
{
	// ���� ������. �ѹ��� �ڵ�ȣ��
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

