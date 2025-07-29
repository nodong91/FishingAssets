using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using UnityEditor.SearchService;

public class SceneLoader : MonoBehaviour
{
    internal static void OnSceneLoaded(string scene, LoadSceneMode mode)
    {
        SceneManager.LoadScene(scene, mode);
    }
}
