using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Option_Manager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Custom_Button closeButton;
    public Custom_Button goTitleButton, goExitButton;
    const string saveData = "SaveOptionData";
    public Data_Option optionData;

    [System.Serializable]
    public struct ScreenStruct
    {
        public Toggle toggle;
        public GameObject screenObject;
    }
    public ScreenStruct[] screenStruct;

    public TranslateLanguage translateLanguage;
    public Audio_Manager audioManager;
    public Quality_Manager qualityManager;
    const string soundName = "Fx_0001";

    public static Option_Manager current;

    private void Awake()
    {
        current = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetStart()
    {
        LoadOption();// 옵션 데이터 로드
        translateLanguage.SetStart();// 컨트롤 쪽 세팅
        audioManager.SetStart();// 오디오 매니저 세팅
        qualityManager.SetStart();// 퀄리티 매니저 세팅

        closeButton.SetButton(delegate { OpenCanvas(false); });
        SetToggle();

        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, false));// 저장 안하고 닫기
    }
    public delegate void DeleCloseOption();
    public DeleCloseOption deleCloseOption;
    public void OpenCanvas(bool _open)
    {
        if (_open == false)
            StaticOpenCanvas.deleEndOpen += EndOpenCanvas;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        Camera_Manager.current?.CameraFocus(_open);
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

        goTitleButton.SetButton(GoTitle, EnterButton);
        goExitButton.SetButton(GoExit, EnterButton);
    }

    void EnterButton(Custom_Button _button)
    {
        Singleton_Audio.INSTANCE.Audio_FX(soundName);
    }

    void InputToggle(int _index)
    {
        screenStruct[_index].screenObject.gameObject.SetActive(screenStruct[_index].toggle.isOn);
    }


    void GoTitle()
    {
        LoadingManager.current.GoTitle();
        Singleton_Audio.INSTANCE.Audio_Environment(null);
        OpenCanvas(false);
    }

    void GoExit()
    {
        LoadingManager.current.GoExit();
    }

    public void SetThemeMusic(string _music)
    {
        if (string.IsNullOrEmpty(_music))
        {
            Singleton_Audio.INSTANCE.Audio_BGM(null);
            return;
        }
        audioManager.PlayBGMAudio(_music);
    }


    public bool TryOptionFile()
    {
        string filePath = Application.dataPath + "/Save/" + saveData + ".json";
        FileInfo fileInfo = new FileInfo(filePath);
        return fileInfo.Exists;
    }







    void SaveOption()
    {
        optionData = new Data_Option
        {
            language = (int)Singleton_Data.INSTANCE.languageType,
            qualityLevel = qualityManager.levelIndex,
            resolutionIndex = qualityManager.resolutionIndex,
            fullScreen = qualityManager.fullScreen,
            frameRateIndex = qualityManager.frameRate,
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
        Static_JsonManager.SaveOptionData(saveData, optionData);
    }

    public void LoadOption()
    {
        if (Static_JsonManager.TryLoadOptionData(saveData, out Data_Option _data))
        {
            optionData = _data;
        }
        else
        {
            optionData = new Data_Option();
            optionData.DefaultOption();
        }
    }
}
