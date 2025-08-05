using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    internal static void OnSceneLoaded(string scene, LoadSceneMode mode)
    {
        SceneManager.LoadScene(scene, mode);
    }

    static IEnumerator LoadSceneTest(string sceneName)
    {
        float deleyTime = 1.0f;
        //Singleton_Manager.INSTANCE.FadeScreen(true);
        yield return new WaitForSeconds(deleyTime);

        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName);
        asyncOper.allowSceneActivation = false;

        float timer = 0.0f;
        while (!asyncOper.isDone)
        {
            timer += Time.deltaTime;

            if (asyncOper.progress < 0.9f)
            {
                //loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, asyncOper.progress, timer);
                //percent.text = ((int)(loadingBar.fillAmount * 100f)).ToString();
                //if (loadingBar.fillAmount >= asyncOper.progress)
                //{
                //    timer = 0.0f;
                //}
            }
            else
            {
                //loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1f, timer);
                //percent.text = ((int)(loadingBar.fillAmount * 100f)).ToString();
                //if (loadingBar.fillAmount == 1.0f)
                //{
                //    yield return new WaitForSeconds(deleyTime);

                //    Singleton_Manager.INSTANCE.FadeScreen(false);
                //    Debug.Log("Loading : " + asyncOper.progress);
                //    yield return new WaitForSeconds(deleyTime = 0.5f);
                //    asyncOper.allowSceneActivation = true;
                //    yield break;
                //}
                asyncOper.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
