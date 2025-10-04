using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using static Data_Manager;

public class Title_Manager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;

    public Option_Manager optionManager;
    public Credit_Rolling creditRolling;
    private Credit_Rolling instCreditRolling;
    const string titleTheme = "BGM_0001";
    const string soundName = "FX_0001";

    const string _continue = "LMn_0001";
    const string _newStart = "LMn_0002";
    const string _credit = "LMn_0003";
    const string _setting = "LMn_0004";
    const string _exit = "LMn_0005";

    public RectTransform selectMask;
    Coroutine enterCoroutine;
    Vector2 originalSize;
    public UI_Popup newGamePopup;
    [Header(" [ Buttons ]")]
    public Custom_Button continueButton;
    public Custom_Button newStartButton, creditButton, settingButton, exitButton;
    public TMPro.TMP_Text continueText, newStartText, creditText, settingText, exitText;
    bool continueEnable;
    public TMPro.TMP_Text volume;

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

    void Start()
    {
        StartCoroutine(SetManager());
        volume.text = LoadingManager.current.volume;

        continueEnable = TryOptionFile();
        Debug.LogWarning($"Option_Manager : {continueEnable}");
        continueButton.gameObject.SetActive(continueEnable);
        continueButton.SetButton(ContinueButton, ActionEnter, ActionExit);
        newStartButton.SetButton(NewStartButton, ActionEnter, ActionExit);
        creditButton.SetButton(CreditButton, ActionEnter, ActionExit);
        settingButton.SetButton(SettingButton, ActionEnter, ActionExit);
        exitButton.SetButton(ExitButton, ActionEnter, ActionExit);

        originalSize = selectMask.sizeDelta;
        ActionExit(null);

        OnTitle();
        SetTime();
    }

    void SetTime()
    {
        RenderSettings.skybox = Instantiate(skyboxMatial);
        Data_Continue data = Singleton_Continue.INSTANCE.LoadContinue();
        if (data != null && (data.hour < 5f || data.hour > 18f))
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

    bool TryOptionFile()
    {
        string filePath = Application.dataPath + "/Save/" + "SaveContinue" + ".json";
        FileInfo fileInfo = new FileInfo(filePath);
        return fileInfo.Exists;
    }

    void TextSetting()
    {
        continueText.text = Singleton_Data.INSTANCE.GetLanguage(_continue);
        newStartText.text = Singleton_Data.INSTANCE.GetLanguage(_newStart);
        creditText.text = Singleton_Data.INSTANCE.GetLanguage(_credit);
        settingText.text = Singleton_Data.INSTANCE.GetLanguage(_setting);
        exitText.text = Singleton_Data.INSTANCE.GetLanguage(_exit);
    }

    void OnTitle()
    {
        TextSetting();
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, true));
    }

    void ActionEnter(Custom_Button _button)
    {
        Singleton_Audio.INSTANCE.Audio_FX(soundName);
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
            // 저장 파일 제거
            GetUIPopup.buttonAction = NewGamePopup;
            GetUIPopup.OpenCanvas(true);
        }
        else
        {
            StartGame();//NewStartButton
        }
    }

    void NewGamePopup(bool _action)
    {
        Debug.Log("NewGamePopup : " + _action);
        if (_action == true)
        {
            StartCoroutine(RemoveSaveFile());
        }
    }

    IEnumerator RemoveSaveFile()
    {
        string path = Application.dataPath + "/Save/";
        string[] allFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (string file in allFiles)
        {
            File.Delete(file);
        }
        Directory.Delete(path, true);
        // 닫힐때 옵션이 저장이 되는데 창 데이터가 있어서 기존 내용이 저장됨
        Option_Manager.current.LoadOption();// 옵션 데이터 리셋
        yield return null;

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

        Option_Manager.current.SetThemeMusic(titleTheme); // 테마 음악 설정
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
        Debug.Log("LoadingComplate");
    }

    private Unit_Player instPlayer;
    Unit_Player GetPlayer
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
