using System.Collections;
using UnityEngine;

public class Title_Manager : MonoBehaviour
{
    public Custom_Button continueButton, newStartButton, loadButton, settingButton, exitButton;
    public Option_Manager optionManager;

    void Start()
    {
        continueButton.deleClicked = ContinueButton;
        newStartButton.deleClicked = NewStartButton;
        loadButton.deleClicked = LoadButton;
        settingButton.deleClicked = SettingButton;
        exitButton.deleClicked = ExitButton;

        if (Option_Manager.current == null)
            Instantiate(optionManager);
        LoadingManager.current.deleComplate = LoadingComplate;// 로딩 완료

        StartCoroutine(MovingUnit());
    }

    void ContinueButton()
    {
        StopAllCoroutines();
        Option_Manager.current.OpenCanvas(false);
        LoadingManager.current.GoMain();
    }

    void NewStartButton()
    {

    }

    void LoadButton()
    {

    }

    void SettingButton()
    {
        Option_Manager.current.OpenCanvas(true);
    }

    void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    [Header(" -Ship")]
    public Transform player;
    public Transform startPoint, endPoint;
    public float speed = 0.1f;
    public Material reflectionMaterial;

    IEnumerator MovingUnit()
    {
        while (true)
        {
            player.gameObject.SetActive(true);
            float normalize = 0f;
            while (normalize < 1f)
            {
                normalize += Time.deltaTime * speed;
                Vector3 startPosition = new Vector3(startPoint.position.x, player.transform.position.y, startPoint.position.z);
                Vector3 endPosition = new Vector3(endPoint.position.x, player.transform.position.y, endPoint.position.z);
                player.transform.position = Vector3.Lerp(startPosition, endPosition, normalize);
                yield return null;

                string shipPosition = "_ShipPosition";
                reflectionMaterial.SetVector(shipPosition, player.position);
            }
            player.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            player.position = startPoint.position;
        }
    }

    void LoadingComplate()
    {
        Debug.LogWarning("LoadingComplate");
    }
}
