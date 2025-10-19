using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader // static 클래스는 static 변수 메서드만 가질수있음
{
    public enum Scene // enum은 static 안됨
    {
        MainMenuScene,
        GameScene,
        LoadingScene,
        LobbyScene,
        CharacterSelectScene
    }

    public static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
		SceneManager.LoadScene(targetScene.ToString());
	}
}
