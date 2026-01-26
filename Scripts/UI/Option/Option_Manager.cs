using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Option_Manager : MonoBehaviour
{
    public bool optionOpen;
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Custom_Button closeButton;
    public Custom_Button saveButton, goTitleButton, goExitButton;
    public Custom_Button resetButton;
    public Data_Option optionData;

    [System.Serializable]
    public struct ScreenStruct
    {
        public Toggle toggle;
        public GameObject screenObject;
    }
    public ScreenStruct[] screenStruct;

    public Option_Language optionLanguage;
    public Option_Control optionControl;
    public Option_Audio optionAudio;
    public Option_Quality optionQuality;
    public CanvasGroup saveOption, saveTextCanvase;
    public TMPro.TMP_Text saveText;

    public delegate void DeleCloseOption();
    public DeleCloseOption deleCloseOption;

    public static Option_Manager current;

    private void Awake()
    {
        current = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetStart()
    {
        LoadOption();// 옵션 데이터 로드
        optionControl.SetStart();// 컨트롤 쪽 세팅
        optionAudio.SetStart();// 오디오 매니저 세팅
        optionQuality.SetStart();// 퀄리티 매니저 세팅
        optionLanguage.SetStart();
        langageDelegate = optionLanguage.SetStart;
        saveTextCanvase.alpha = 0f;
        closeButton.SetButton(CloseCanvas);
        SetToggle();
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));// 저장 안하고 닫기
    }

    public void CloseCanvas()
    {
        if (Game_Manager.current == null)
        {
            OpenCanvas(false);
        }
        else
        {
            Game_Manager.current.GetMainUI?.CloseOption();
        }
    }

    public void OpenCanvas(bool _open)
    {
        if (optionOpen == _open || isChange == true)
            return;

        optionOpen = _open;
        if (_open == false)// 닫힐 때
            StaticOpenCanvas.deleEndOpen += EndOpenCanvas;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        if (_open == true)
        {
            screenStruct[0].toggle.isOn = true;
        }
        else if (deleCloseOption != null)
        {
            deleCloseOption.Invoke();
            deleCloseOption = null;
        }
    }

    void EndOpenCanvas()
    {
        StaticOpenCanvas.deleEndOpen -= EndOpenCanvas;
        SaveOption();
    }

    void SetToggle()
    {
        for (int i = 0; i < screenStruct.Length; i++)
        {
            int index = i;
            screenStruct[i].toggle.onValueChanged.AddListener(delegate { InputToggle(index); });
            InputToggle(index);
        }
        resetButton.SetButton(SetDefaultButton, EnterButton);
        saveButton.SetButton(SaveGame, EnterButton);
        goTitleButton.SetButton(GoTitle, EnterButton);
        goExitButton.SetButton(GoExit, EnterButton);
    }

    void EnterButton(Custom_Button _button)
    {
        Singleton_Audio.INSTANCE.Audio_FX(Const_Audio._buttonClick);
    }

    void InputToggle(int _index)
    {
        bool onTitle = LoadingManager.current.currentScene != LoadingManager.CurrentScene.Title;
        saveButton.gameObject.SetActive(onTitle);
        goTitleButton.gameObject.SetActive(onTitle);
        screenStruct[_index].screenObject.gameObject.SetActive(screenStruct[_index].toggle.isOn);
    }

    void SetDefaultButton()
    {
        SetDefault();

        optionControl.SetStart();// 컨트롤 쪽 세팅
        optionAudio.SetStart();// 오디오 매니저 세팅
        optionQuality.SetStart();// 퀄리티 매니저 세팅
    }

    void SaveGame()
    {
        if (isChange == true)
            return;
        StartCoroutine(SetSaving(3.0f));
    }

    bool isChange = false;
    void GoTitle()
    {
        if (isChange == true)
            return;
        Singleton_Audio.INSTANCE.Audio_Environment(null);
        LoadingManager.current.GoTitle();
        OpenCanvas(false);
    }

    void GoExit()
    {
        if (isChange == true)
            return;
        StartCoroutine(SetSaving(3.0f, LoadingManager.current.GoExit));
    }

    IEnumerator SetSaving(float _delay, Action _action = null)
    {
        isChange = true;
        Singleton_Continue.INSTANCE.SaveContinue();// 게임 종료

        saveOption.interactable = false;
        saveOption.blocksRaycasts = false;
        saveText.text = "저장 중...";
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime * 3f;
            saveOption.alpha = 1f - normalize;
            saveTextCanvase.alpha = normalize;
            yield return null;
        }
        yield return new WaitForSeconds(_delay);

        saveText.text = "저장 완료";
        yield return new WaitForSeconds(1f);

        if(_action == null)
        {
            normalize = 0f;
            while (normalize < 1f)
            {
                normalize += Time.deltaTime * 3f;
                saveOption.alpha = normalize;
                saveTextCanvase.alpha = 1f - normalize;
                yield return null;
            }
        }

        saveOption.interactable = true;
        saveOption.blocksRaycasts = true;
        isChange = false;
        _action?.Invoke();
    }

    public void SetThemeMusic(string _music)
    {
        Debug.LogWarning("SetThemeMusic : " + _music);
        if (string.IsNullOrEmpty(_music))
        {
            Singleton_Audio.INSTANCE.Audio_BGM(null);
            return;
        }
        optionAudio.PlayBGMAudio(_music);
    }

    void SaveOption()
    {
        optionData = new Data_Option
        {
            setFPS = optionControl.GetFPS,
            shake = optionControl.GetShake,
            cursorLock = optionControl.GetCursor,
            language = (int)Singleton_Data.INSTANCE.languageType,
            qualityLevel = optionQuality.levelIndex,
            resolutionIndex = optionQuality.resolutionIndex,
            fullScreen = optionQuality.fullScreen,
            frameRateIndex = optionQuality.frameRate,
            audioStruct = new Data_Option.AudioStruct
            {
                masterMute = Singleton_Audio.INSTANCE.masterMute,
                masterVolume = Singleton_Audio.INSTANCE.masterVolume,
                bgmMute = Singleton_Audio.INSTANCE.bgmMute,
                bgmVolume = Singleton_Audio.INSTANCE.bgmVolume,
                fxMute = Singleton_Audio.INSTANCE.fxMute,
                fxVolume = Singleton_Audio.INSTANCE.fxVolume,
                envMute = Singleton_Audio.INSTANCE.envMute,
                envVolume = Singleton_Audio.INSTANCE.envVolume,
            },
        };
        Debug.LogWarning("옵션 저장");
        Static_JsonManager.SaveOptionData(Const_Save._option, optionData);
    }

    public void LoadOption()
    {
        if (Static_JsonManager.TryLoadOptionData(Const_Save._option, out Data_Option _data))
        {
            optionData = _data;
        }
        else
        {
            SetDefault();
        }
    }

    void SetDefault()
    {
        optionData = new Data_Option();
        optionData.DefaultOption();

        Static_JsonManager.SaveOptionData(Const_Save._option, optionData);
    }

    public delegate void LangageDelegate();
    public LangageDelegate langageDelegate;

    public void SetLangage()
    {

    }
}
