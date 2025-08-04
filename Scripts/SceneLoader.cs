using UnityEngine.SceneManagement;

public static class SceneLoader 
{
    internal static void OnSceneLoaded(string scene, LoadSceneMode mode)
    {
        SceneManager.LoadScene(scene, mode);
    }
}
