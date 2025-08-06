using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public string[] sceneNames;
    List<AsyncOperation> asyncOperation;
    public Image background;
    bool open;
    int complateIndex;

    public static LoadingManager current;

    private void Awake()
    {
        current = this;
        DontDestroyOnLoad(gameObject);
    }

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

        complateIndex = 0;
        asyncOperation = new List<AsyncOperation>();
        for (int i = 0; i < sceneNames.Length; i++)
        {
            asyncOperation.Add(SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Additive));
            StartCoroutine(LoadingScene(i));
        }
    }

    public void CloseLoading()
    {
        if (complateIndex == asyncOperation.Count)
        {
            for (int i = 0; i < asyncOperation.Count; i++)
            {
                asyncOperation[i].allowSceneActivation = true;
            }
            StartCoroutine(CloseSetting());
        }
    }

    IEnumerator CloseSetting()
    {
        deleComplate?.Invoke();
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
            background.gameObject.SetActive(alpha > 0);
            yield return null;
        }
    }

    IEnumerator LoadingScene(int _index)
    {
        asyncOperation[_index].allowSceneActivation = false;
        //while (!async[_index].isDone)
        bool loading = true;
        while (loading == true)
        {
            if (asyncOperation[_index].progress == 0.9f)
            {
                loading = false;
            }
            yield return null;
            Debug.LogWarning(sceneNames[_index] + asyncOperation[_index].progress);
        }

        // 완료 체크
        Debug.LogWarning(sceneNames[_index]);
        complateIndex++;
        if (complateIndex == asyncOperation.Count)
        {
            for (int i = 0; i < asyncOperation.Count; i++)
            {
                asyncOperation[i].allowSceneActivation = true;
            }
            StartCoroutine(CloseSetting());
        }
    }

    public delegate void DeleComplate();
    public DeleComplate deleComplate;

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