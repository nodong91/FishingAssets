using System.Collections;
using UnityEngine;

public class Title_Manager : MonoBehaviour
{
    public Custom_Button continueButton, newStartButton, creditButton, settingButton, exitButton;
    public Option_Manager optionManager;
    public Credit_Rolling creditRolling;
    private Credit_Rolling instCreditRolling;
    public string titleTheme = "Main Theme";
    public string soundName = "pop-39222";

    void Start()
    {
        continueButton.SetButton(ContinueButton, EnterButton);
        newStartButton.SetButton(NewStartButton, EnterButton);
        creditButton.SetButton(CreditButton, EnterButton);
        settingButton.SetButton(SettingButton, EnterButton);
        exitButton.SetButton(ExitButton, EnterButton);

        StartCoroutine(SetManager());
    }

    void EnterButton()
    {
        Singleton_Audio.INSTANCE.Audio_FX(soundName);
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

    void CreditButton()
    {
        if (instCreditRolling == null)
            instCreditRolling = Instantiate(creditRolling);
        instCreditRolling.OpenCanvas(true);
    }

    void SettingButton()
    {
        Option_Manager.current.OpenCanvas(true);
    }

    void ExitButton()
    {
        LoadingManager.current.GoExit();
    }

    [Header(" [ Ship ]")]
    public Transform player;
    public Transform startPoint, endPoint;
    public float speed = 0.1f;
    public Material reflectionMaterial;

    IEnumerator SetManager()
    {
        if (Option_Manager.current == null)
            Instantiate(optionManager);

        if (LoadingManager.current != null)
            LoadingManager.current.deleComplate = LoadingComplate;// 로딩 완료
        yield return null;

        Option_Manager.current.SetThemeMusic(titleTheme); // 테마 음악 설정
        while (true)// Loop the movement
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
        Debug.Log("LoadingComplate");
    }
}
