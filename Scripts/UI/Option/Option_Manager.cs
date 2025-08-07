using UnityEngine;
using UnityEngine.UI;
using static Data_Manager;

public class Option_Manager : MonoBehaviour
{
    public StaticOpenCanvas.CanvasStruct[] canvasStructs;
    public Button closeButton;
    public Custom_Button goTitleButton;
    const string saveData = "SaveOptionData";
    public Data_Option optionData;

    public Audio_Manager audioManager;

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
        closeButton.onClick.AddListener(delegate { OpenCanvas(false); });
        SetToggle();

        OpenCanvas(false);
    }

    public void OpenCanvas(bool _open)
    {
        StaticOpenCanvas.deleEndOpen = EndOpenCanvas;
        StartCoroutine(StaticOpenCanvas.OpenCanvas(canvasStructs, _open));
        if (_open == true)
            toggles[0].isOn = true;
    }

    void EndOpenCanvas()
    {
        SaveOption();
    }
    public Toggle[] toggles;
    public GameObject[] test;
    void SetToggle()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener(delegate { InputToggle(index); });
        }
        //toggles[0].isOn = true;
        goTitleButton.deleClicked = GoTitle;
    }

    void InputToggle(int _index)
    {
        test[_index].gameObject.SetActive(toggles[_index].isOn);
    }


    void GoTitle()
    {
        LoadingManager.current.GoTitle();
        OpenCanvas(false);
    }












    void SaveOption()
    {
        optionData = new Data_Option
        {
            audioStruct = new Data_Option.AudioStruct
            {
                bgmMute = Singleton_Audio.INSTANCE.bgmMute,
                bgmVolume = Singleton_Audio.INSTANCE.bgmVolume,
                fxMute = Singleton_Audio.INSTANCE.fxMute,
                fxVolume = Singleton_Audio.INSTANCE.fxVolume,
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
                },

            };
        }
    }
}
