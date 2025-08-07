using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public string[] currentNames;
    public string[] sceneNames;
    List<AsyncOperation> asyncOperation;
    public Image background;
    bool open;
    int complateIndex;

    public static LoadingManager current;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        GoTitle();
    }


    public void GoTitle()
    {
        sceneNames = new string[1];
        sceneNames[0] = "Title";

        OpenLoading();
    }

    public void GoMain()
    {
        sceneNames = new string[2];
        sceneNames[0] = "SampleScene";
        sceneNames[1] = "Fishing";

        OpenLoading();
    }

    void OpenLoading()
    {
        Singleton_Controller.INSTANCE.ResetDefault();
        StartCoroutine(StartLoading());
    }

    IEnumerator StartLoading()
    {
        yield return StartCoroutine(OpenScreen(true));
        yield return StartCoroutine(UnloadScene());

        complateIndex = 0;
        asyncOperation = new List<AsyncOperation>();
        for (int i = 0; i < sceneNames.Length; i++)
        {
            asyncOperation.Add(SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Additive));
            StartCoroutine(LoadingScene(i));
        }
    }

    IEnumerator OpenScene(List<AsyncOperation> _async)
    {
        complateIndex++;
        if (complateIndex == _async.Count)
        {
            for (int i = 0; i < _async.Count; i++)
            {
                _async[i].allowSceneActivation = true;
            }
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(OpenScreen(false));
    }

    IEnumerator OpenScreen(bool _open)
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
        yield return StartCoroutine(OpenScene(asyncOperation));

        currentNames = sceneNames;
        deleComplate?.Invoke();
    }

    public delegate void DeleComplate();
    public DeleComplate deleComplate;

    IEnumerator UnloadScene()
    {
        for (int i = 0; i < currentNames.Length; i++)
        {
            var sceneName = SceneManager.GetSceneByName(currentNames[i]);
            if (sceneName.isLoaded)
            {
                var unloadScene = SceneManager.UnloadSceneAsync(currentNames[i]);
                while (!unloadScene.isDone)
                {
                    yield return null;
                }
            }
        }
    }
}