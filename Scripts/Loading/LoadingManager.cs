using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private string[] currentNames;
    private string[] sceneNames;
    public RectTransform background;
    public Loading_Hint hint;

    public static LoadingManager current;

    private void Awake()
    {
        current = this;
    }
    private void Start()
    {
        hint.SetStart();
        GoTitle();
    }

    public void GoTitle()
    {
        sceneNames = new string[1] { Const_Scene._title };
        OpenLoading();
    }

    public void GoMain()
    {
        sceneNames = new string[2] { Const_Scene._gameManager, Const_Scene._islandMain };
        OpenLoading();
    }

    public void GoTest()
    {
        sceneNames = new string[1] { Const_Scene._test };
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
        hint.SetHint();
        yield return StartCoroutine(OpenScreen(true));
        yield return StartCoroutine(UnloadScene());

        for (int i = 0; i < sceneNames.Length; i++)
        {
            AsyncOperation _asyncOperation = SceneManager.LoadSceneAsync(sceneNames[i], LoadSceneMode.Additive);
            while (_asyncOperation.progress < 0.9f)
            {
                yield return null;
            }
            _asyncOperation.allowSceneActivation = true;
        }

        // 완료 체크
        yield return new WaitForSeconds(2.5f);// 완료 후 잠시 대기
        yield return StartCoroutine(OpenScreen(false));
        deleComplate?.Invoke();
        currentNames = sceneNames;
    }

    public CanvasGroup alphaCanvas;
    IEnumerator OpenScreen(bool _open)
    {
        float prevHight, targetHight, targetAlpha;
        if (_open == true)
        {
            prevHight = -(Screen.height + 100);
            targetHight = 0f;
            targetAlpha = 1f;
        }
        else
        {
            prevHight = 0f;
            targetHight = Screen.height + 100;
            targetAlpha = 0f;
        }

        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.unscaledDeltaTime * 1.5f;
            float hight = Mathf.Lerp(prevHight, targetHight, normalize);
            background.anchoredPosition = new Vector2(0f, hight);

            float alpha = Mathf.Lerp(1f - targetAlpha, targetAlpha, normalize);
            background.gameObject.SetActive(alpha > 0);
            alphaCanvas.alpha = alpha;
            yield return null;
        }
    }

    public delegate void DeleComplate();
    public DeleComplate deleComplate;

    IEnumerator UnloadScene()// 기존 씬 제거
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