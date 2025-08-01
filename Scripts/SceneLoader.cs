using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    internal static void OnSceneLoaded(string scene, LoadSceneMode mode)
    {
        SceneManager.LoadScene(scene, mode);
    }
}
