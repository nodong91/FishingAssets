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
        continueButton.SetButton(ContinueButton, ActionEnter, ActionExit);
        newStartButton.SetButton(NewStartButton, ActionEnter, ActionExit);
        creditButton.SetButton(CreditButton, ActionEnter, ActionExit);
        settingButton.SetButton(SettingButton, ActionEnter, ActionExit);
        exitButton.SetButton(ExitButton, ActionEnter, ActionExit);

        originalSize = selectMask.sizeDelta;
        ActionExit();

        StartCoroutine(SetManager());
    }

    void ActionEnter(GameObject _button)
    {
        Singleton_Audio.INSTANCE.Audio_FX(soundName);
        selectMask.gameObject.SetActive(true);
        selectMask.position = _button.transform.position;

        if (enterCoroutine != null)
            StopCoroutine(enterCoroutine);
        enterCoroutine = StartCoroutine(ActingEnter());
    }
    public RectTransform selectMask;
    Coroutine enterCoroutine;
    Vector2 originalSize;
    IEnumerator ActingEnter()
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 5f;
            selectMask.sizeDelta = Vector2.Lerp(new Vector2(0, originalSize.y), originalSize, normalize);
            yield return null;
        }
    }

    void ActionExit()
    {
        selectMask.gameObject.SetActive(false);
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
    public Unit_Player player;
    public Transform startPoint, endPoint;
    public float speed = 0.1f;
    public Material reflectionMaterial;

    IEnumerator SetManager()
    {
        if (Option_Manager.current == null)
        {
            Instantiate(optionManager);
            Option_Manager.current.SetStart();
        }

        if (LoadingManager.current != null)
            LoadingManager.current.deleComplate = LoadingComplate;// 로딩 완료
        yield return null;

        Option_Manager.current.SetThemeMusic(titleTheme); // 테마 음악 설정
        while (true)// Loop the movement
        {
            GetPlayer.gameObject.SetActive(true);
            float normalize = 0f;
            while (normalize < 1f)
            {
                normalize += Time.deltaTime * speed;
                Vector3 startPosition = new Vector3(startPoint.position.x, GetPlayer.transform.position.y, startPoint.position.z);
                Vector3 endPosition = new Vector3(endPoint.position.x, GetPlayer.transform.position.y, endPoint.position.z);
                GetPlayer.transform.position = Vector3.Lerp(startPosition, endPosition, normalize);
                yield return null;

                string shipPosition = "_ShipPosition";
                reflectionMaterial.SetVector(shipPosition, GetPlayer.transform.position);
            }
            GetPlayer.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            GetPlayer.transform.position = startPoint.position;
        }
    }

    void LoadingComplate()
    {
        Debug.Log("LoadingComplate");
    }

    private Unit_Player instPlayer;
    public Unit_Player GetPlayer
    {
        get
        {
            if (instPlayer == null)
            {
                instPlayer = Instantiate(player, transform);
            }
            return instPlayer;
        }
    }
}
