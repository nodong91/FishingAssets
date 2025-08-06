using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public string[] sceneNames;
    List<AsyncOperation> async;
    public float value;
    public Image background;
    bool open;

    private void Start()
    {
        StartCoroutine(SetScreen(false));
    }

    public void OpenLoading()
    {
        StartCoroutine(StartLoading());
    }

    IEnumerator StartLoading()
    {
        yield return StartCoroutine(SetScreen(true));

        value = 0;
        async = new List<AsyncOperation>();
        for (int i = 0; i < sceneNames.Length; i++)
        {
            async.Add(SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Additive));
            StartCoroutine(LoadSceneTest(i));
        }
    }

    public void CloseLoading()
    {
        if (value == async.Count)
        {
            for (int i = 0; i < async.Count; i++)
            {
                async[i].allowSceneActivation = true;
            }
            StartCoroutine(CloseSetting());
        }
    }

    IEnumerator CloseSetting()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SetScreen(false));
    }

    IEnumerator SetScreen(bool _open)
    {
        open = _open;

        float targetAlpha = open == false ? 0f : 1f;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 10f;
            float alpha = Mathf.Lerp(1f - targetAlpha, targetAlpha, normalize);
            background.material.SetFloat("_Normalize", alpha);
            yield return null;
            background.gameObject.SetActive(alpha > 0);
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

    public void Unloading()
    {
        StartCoroutine(UnloadScene());
    }

    IEnumerator UnloadScene()
    {
        for (int i = 0; i < sceneNames.Length; i++)
        {
            var sceneName = SceneManager.GetSceneByName(sceneNames[i]);
            if (sceneName.isLoaded)
            {
                var unloadScene = SceneManager.UnloadSceneAsync(sceneNames[i]);
                while (!unloadScene.isDone)
                {
                    yield return null;
                }
            }
        }
    }
}