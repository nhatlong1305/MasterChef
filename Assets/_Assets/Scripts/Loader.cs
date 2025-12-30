using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        GameMenuScenes,   // MENU
        LoadingScenes,    // LOADING
        Kitchen           // GAME
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScenes.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
