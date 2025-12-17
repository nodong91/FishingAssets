using System.Collections;
using System.IO;
using UnityEngine;
using static Data_Manager;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Title_Manager : MonoBehaviour
{
    public bool isNewGame = false;
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;

    public Option_Manager optionManager;
    public Credit_Rolling creditRolling;
    private Credit_Rolling instCreditRolling;

    public RectTransform selectMask;
    Coroutine enterCoroutine;
    Vector2 originalSize;
    public UI_Popup newGamePopup;
    [Header(" [ Buttons ]")]
    public Custom_Button continueButton;
    public Custom_Button newStartButton, creditButton, settingButton, exitButton;
    public Custom_Button testButton;
    public TMPro.TMP_Text continueText, newStartText, creditText, settingText, exitText;
    public bool continueEnable;

    public Light DayLight;
    public Color nightColor;
    [ColorUsage(true, true)]
    public Color emissionColor;
    public Material skyboxMatial;

    [Header(" [ Ship ]")]
    public Unit_Player player;
    public Transform startPoint, endPoint;
    public float speed = 0.1f;
    public Material reflectionMaterial;
    bool isMove = false;
    Data_Continue continueData;

    void Start()
    {
        StartCoroutine(SetManager());

        continueEnable = TryContinue();
        continueButton.gameObject.SetActive(continueEnable);
        continueButton.SetButton(ContinueButton, ActionEnter, ActionExit);
        newStartButton.SetButton(NewStartButton, ActionEnter, ActionExit);
        creditButton.SetButton(CreditButton, ActionEnter, ActionExit);
        settingButton.SetButton(SettingButton, ActionEnter, ActionExit);
        exitButton.SetButton(ExitButton, ActionEnter, ActionExit);
        testButton.SetButton(TestScene, ActionEnter, ActionExit);
        //Debug.LogWarning($"Option_Manager : {continueEnable}");

        originalSize = selectMask.sizeDelta;
        ActionExit(null);

        OnTitle();
        SetTime();
    }

    bool TryContinue()
    {
        // 컨티뉴 파일과 옵션파일이 모두 존재해야 컨티뉴 가능
        bool continueData = Static_JsonManager.TryLoadCountinueData(Const_Save._continue, out Data_Continue _dataContinue);
        bool optionData = Static_JsonManager.TryLoadOptionData(Const_Save._option, out Data_Option _dataOption);

        return continueData && optionData;
    }

    void TestScene()
    {
        LoadingManager.current.GoTest();
    }

    void SetTime()
    {
        RenderSettings.skybox = Instantiate(skyboxMatial);
        continueData = Singleton_Continue.INSTANCE.LoadContinue();
        if (continueData == null)
            return;

        if (continueData != null && (continueData.hour < 5f || continueData.hour > 18f))
        {
            // 밤
            DayLight.color = nightColor;
            Shader.SetGlobalColor("_EmissionColor", emissionColor);
            RenderSettings.skybox.SetFloat("_Amount", 1f);
        }
        else
        {
            // 낮
            DayLight.color = Color.white;
            Shader.SetGlobalColor("_EmissionColor", Color.black);
            RenderSettings.skybox.SetFloat("_Amount", 0f);
        }
    }

    void TextSetting()
    {
        continueText.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._continue);
        newStartText.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._newStart);
        creditText.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._credit);
        settingText.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._setting);
        exitText.text = Singleton_Data.INSTANCE.GetLanguage(Const_ETC._exit);
    }

    void OnTitle()
    {
        TextSetting();
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, true));
    }

    void ActionEnter(Custom_Button _button)
    {
        Singleton_Audio.INSTANCE.Audio_FX(Const_Audio._buttonClick);
        selectMask.gameObject.SetActive(true);
        selectMask.position = _button.transform.position;

        if (enterCoroutine != null)
            StopCoroutine(enterCoroutine);
        enterCoroutine = StartCoroutine(ActingEnter());
    }

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

    void ActionExit(Custom_Button _button)
    {
        selectMask.gameObject.SetActive(false);
    }

    void ContinueButton()
    {
        StartGame();//ContinueButton
    }

    void StartGame()
    {
        //StopAllCoroutines();
        isMove = false;
        Option_Manager.current.OpenCanvas(false);
        LoadingManager.current.GoMain();
    }

    private UI_Popup instNewGamePopup;
    UI_Popup GetUIPopup
    {
        get
        {
            if (instNewGamePopup == null)
            {
                instNewGamePopup = Instantiate(newGamePopup, transform);
                instNewGamePopup.SetStart();
            }
            return instNewGamePopup;
        }
    }

    void NewStartButton()
    {
        if (continueEnable == true)
        {
            // 저장 파일 제거 팝업
            GetUIPopup.buttonAction = NewGamePopup;
            GetUIPopup.OpenCanvas(true);
        }
        else
        {
            NewGamePopup(true);
        }
    }

    void NewGamePopup(bool _action)
    {
        Debug.Log("NewGamePopup : " + _action);
        if (_action == true)
        {
            StartCoroutine(SetNewGame());
        }
    }

    IEnumerator SetNewGame()
    {
        yield return StartCoroutine(Static_JsonManager.RemoveSaveFile());// 타이틀에서 새로 시작
        StartGame();//RemoveSaveFile
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
        Option_Manager.current.deleCloseOption = OnTitle;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));
    }

    void ExitButton()
    {
        LoadingManager.current.GoExit();
    }

    IEnumerator SetManager()
    {
        Camera_Manager.current.SetCameraManager();
        Camera_Manager.current.SetOrbitalTitle();

        if (Option_Manager.current == null)
        {
            Instantiate(optionManager);
            Option_Manager.current.SetStart();
        }

        if (LoadingManager.current != null)
            LoadingManager.current.deleComplate = LoadingComplate;// 로딩 완료
        yield return null;

        isMove = true;
        while (isMove == true)// Loop the movement
        {
            GetPlayer.gameObject.SetActive(true);
            float normalize = 0f;
            while (normalize < 1f)
            {
                normalize += Time.deltaTime * speed;
                Vector3 startPosition = new Vector3(startPoint.position.x, GetPlayer.transform.position.y, startPoint.position.z);
                Vector3 endPosition = new Vector3(endPoint.position.x, GetPlayer.transform.position.y, endPoint.position.z);
                GetPlayer.transform.position = Vector3.Lerp(startPosition, endPosition, normalize);
                GetPlayer.transform.rotation = Quaternion.Slerp(GetPlayer.transform.rotation, Quaternion.LookRotation(endPosition - startPosition), Time.deltaTime * speed * 10f);
                yield return null;

                string shipPosition = "_ShipPosition";
                reflectionMaterial.SetVector(shipPosition, GetPlayer.transform.position);
            }
            GetPlayer.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.0f);

            GetPlayer.transform.position = startPoint.position;
        }
    }

    void LoadingComplate()
    {
        Debug.Log("타이틀 로딩 완료");
        Option_Manager.current.SetThemeMusic(Const_Audio._titleTheme); // 타이틀 음악 시작
    }

    private Unit_Player instPlayer;
    Unit_Player GetPlayer
    {
        get
        {
            if (instPlayer == null)
            {
                instPlayer = Instantiate(player, transform);
                if (Singleton_Data.INSTANCE.Dict_Ship.ContainsKey(continueData.shipData))
                {
                    Data_Ship shipData = Singleton_Data.INSTANCE.Dict_Ship[continueData.shipData];
                    instPlayer.SetShip(shipData);
                }
            }
            return instPlayer;
        }
    }
}
