using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class LoadingManager : MonoBehaviour
{
    public string[] sceneNames;
    List<AsyncOperation> async;
    public float value;

    private void Start()
    {
        value = 0;
        async = new List<AsyncOperation>();
        for (int i = 0; i < sceneNames.Length; i++)
        {
            async.Add(SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Additive));
            StartCoroutine(LoadSceneTest(i));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (value == async.Count)
            {
                for (int i = 0; i < async.Count; i++)
                {
                    async[i].allowSceneActivation = true;
                }
            }
        }
    }

    IEnumerator LoadSceneTest(int _index)
    {
        async[_index].allowSceneActivation = false;
        //while (!async[_index].isDone)
        bool loading = true;
        while (loading == true)
        {
            value += async[_index].progress / 0.9f;
            if (async[_index].progress == 0.9f)
            {
                loading = false;
            }
            yield return null;
        }
    }
}