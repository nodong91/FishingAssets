using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private string[] currentNames;
    private string[] sceneNames;
    List<AsyncOperation> asyncOperation;
    public RectTransform background;
    int complateIndex;
    public string volume;

    const string Title = "Title";
    const string GameManager = "GameManager";
    const string Island_Main = "Island_Main";
    const string Test = "Test";

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
        sceneNames = new string[1] { Title };
        OpenLoading();
    }

    public void GoMain()
    {
        sceneNames = new string[2] { GameManager, Island_Main };
        OpenLoading();
    }

    public void GoTest()
    {
        sceneNames = new string[1] { Test };
        OpenLoading();
    }

    public void GoExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
            yield return new WaitForSeconds(2.5f);// 완료 후 잠시 대기
            yield return StartCoroutine(OpenScreen(false));
            deleComplate?.Invoke();
        }
    }

    IEnumerator OpenScreen(bool _open)
    {
        float prevHight = _open == true ? -(Screen.height + 100) : 0f;
        float targetHight = _open == true ? 0f : Screen.height + 100;
        float targetAlpha = _open == true ? 1f : 0f;
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 1.5f;
            float alpha = Mathf.Lerp(1f - targetAlpha, targetAlpha, normalize);
            float hight = Mathf.Lerp(prevHight, targetHight, normalize);
            background.anchoredPosition = new Vector2(0f, hight);
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
        }
        Debug.Log(sceneNames[_index] + asyncOperation[_index].progress);

        // 완료 체크
        yield return StartCoroutine(OpenScene(asyncOperation));

        currentNames = sceneNames;
    }

    public delegate void DeleComplate();
    public DeleComplate deleComplate;

    IEnumerator UnloadScene()
    {
        if (currentNames == null || currentNames.Length == 0)
        {
            yield break;
        }

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