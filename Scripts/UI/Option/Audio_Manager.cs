using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager.Data_Option;

public class Audio_Manager : MonoBehaviour
{
    public Button prevButton, nextButton;
    public TMP_Text audioText;
    public int currentAudio;
    public string[] audioStrings;
    public Slider BGM_Slider, FX_Slider;
    public Toggle bgm_Mute, fx_Mute;

    public void SetStart()
    {
        AudioStruct audioStruct = Option_Manager.current.optionData.audioStruct;

        prevButton.onClick.AddListener(delegate { NextButton(-1); });
        nextButton.onClick.AddListener(delegate { NextButton(1); });
        BGM_Slider.onValueChanged.AddListener(BGMVolume);
        FX_Slider.onValueChanged.AddListener(FXVolume);
        bgm_Mute.onValueChanged.AddListener(BGMMute);
        fx_Mute.onValueChanged.AddListener(FXMute);

        // UI ¼¼ÆÃ
        BGMVolume(audioStruct.bgmVolume);
        BGMMute(audioStruct.bgmMute);
        FXVolume(audioStruct.fxVolume);
        FXMute(audioStruct.fxMute);

        PlayBGMAudio();
    }

    void NextButton(int _index)
    {
        currentAudio += _index;
        if (currentAudio >= audioStrings.Length)
        {
            currentAudio = 0;
        }
        else if (currentAudio < 0)
        {
            currentAudio = audioStrings.Length - 1;
        }
        PlayBGMAudio();
    }

    void PlayBGMAudio()
    {
        audioText.text = audioStrings[currentAudio];
        Singleton_Audio.INSTANCE.Audio_BGM(audioStrings[currentAudio]);
    }

    void BGMVolume(float _value)
    {
        Singleton_Audio.INSTANCE.SetBGMVolume(_value);
        BGM_Slider.value = _value;
    }

    void BGMMute(bool _isOn)
    {
        Singleton_Audio.INSTANCE.SetBGMMute(_isOn);
        bgm_Mute.isOn = _isOn;
    }

    void FXVolume(float _value)
    {
        Singleton_Audio.INSTANCE.SetFXVolume(_value);
        FX_Slider.value = _value;
    }
    void FXMute(bool _isOn)
    {
        Singleton_Audio.INSTANCE.SetFXMute(_isOn);
        fx_Mute.isOn = _isOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Singleton_Audio.INSTANCE.Audio_FX("pop-39222");
        }
    }
}
