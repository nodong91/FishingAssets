using System;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Option_Manager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Button closeButton;
    public Custom_Button goTitleButton, goExitButton;
    const string saveData = "SaveOptionData";
    public Data_Option optionData;
    public Toggle[] toggles;
    public GameObject[] test;

    public Audio_Manager audioManager;
    public Quality_Manager qualityManager;
    public UI_QuestManager questManager;
    public UI_QuestManager GetQuestManager => questManager;

    public static Option_Manager current;

    private void Awake()
    {
        current = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadOption();

        audioManager.SetStart();
        qualityManager.SetStart();
        questManager.SetStart();

        closeButton.onClick.AddListener(delegate { OpenCanvas(false); });
        SetToggle();

        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        StaticOpenCanvas.deleEndOpen = EndOpenCanvas;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        if (_open == true)
        {
            toggles[0].isOn = true;
            questManager.OpenManager();
        }
    }

    void EndOpenCanvas()
    {
        SaveOption();
    }

    void SetToggle()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener(delegate { InputToggle(index); });
            InputToggle(index);
        }

        goTitleButton.SetButton(GoTitle, EnterButton);
        goExitButton.SetButton(GoExit, EnterButton);
    }

    void EnterButton()
    {
        string soundName = "pop-39222";
        Singleton_Audio.INSTANCE.Audio_FX(soundName);
    }

    void InputToggle(int _index)
    {
        test[_index].gameObject.SetActive(toggles[_index].isOn);
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










    void SaveOption()
    {
        optionData = new Data_Option
        {
            qualityLevel = QualitySettings.GetQualityLevel(),
            audioStruct = new Data_Option.AudioStruct
            {
                bgmMute = Singleton_Audio.INSTANCE.bgmMute,
                bgmVolume = Singleton_Audio.INSTANCE.bgmVolume,
                fxMute = Singleton_Audio.INSTANCE.fxMute,
                fxVolume = Singleton_Audio.INSTANCE.fxVolume,
                envMute = Singleton_Audio.INSTANCE.envMute,
                envVolume = Singleton_Audio.INSTANCE.envVolume,
            },
        };
        Static_JsonManager.SaveOptionData(saveData, optionData);
    }

    void LoadOption()
    {
        if (Static_JsonManager.TryLoadOptionData(saveData, out Data_Option _data))
        {
            optionData = _data;
        }
        else
        {
            optionData = new Data_Option
            {
                audioStruct = new Data_Option.AudioStruct
                {
                    bgmMute = false,
                    bgmVolume = 1f,
                    fxMute = false,
                    fxVolume = 1f,
                    envMute = false,
                    envVolume = 1f,
                },

            };
        }
    }
}
