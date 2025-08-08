using System;
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
    public Slider BGM_Slider, FX_Slider, env_Slider;
    public Toggle bgm_Mute, fx_Mute, env_Mute;

    public void SetStart()
    {
        AudioStruct audioStruct = Option_Manager.current.optionData.audioStruct;

        prevButton.onClick.AddListener(delegate { NextButton(-1); });
        nextButton.onClick.AddListener(delegate { NextButton(1); });

        BGM_Slider.onValueChanged.AddListener(BGMVolume);
        bgm_Mute.onValueChanged.AddListener(BGMMute);
        FX_Slider.onValueChanged.AddListener(FXVolume);
        fx_Mute.onValueChanged.AddListener(FXMute);
        env_Slider.onValueChanged.AddListener(EnvVolume);
        env_Mute.onValueChanged.AddListener(EnvMute);

        // UI ¼¼ÆÃ
        BGMVolume(audioStruct.bgmVolume);
        BGMMute(audioStruct.bgmMute);
        FXVolume(audioStruct.fxVolume);
        FXMute(audioStruct.fxMute);
        EnvVolume(audioStruct.envVolume);
        EnvMute(audioStruct.envMute);
    }

    void NextButton(int _index)
    {
        int index = currentAudio + _index;
        if (index >= audioStrings.Length)
        {
            index = 0;
        }
        else if (index < 0)
        {
            index = audioStrings.Length - 1;
        }
        PlayBGMAudio(index);
    }

    public void PlayBGMAudio(int _index)
    {
        Debug.LogWarning($"BGM - {audioStrings[_index]}");
        currentAudio = _index;
        audioText.text = audioStrings[_index];
        Singleton_Audio.INSTANCE.Audio_BGM(audioStrings[_index]);
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

    void EnvVolume(float _value)
    {
        Singleton_Audio.INSTANCE.SetEnvironmentVolume(_value);
        env_Slider.value = _value;
    }

    void EnvMute(bool _isOn)
    {
        Singleton_Audio.INSTANCE.SetEnvironmentMute(_isOn);
        env_Mute.isOn = _isOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Singleton_Audio.INSTANCE.Audio_FX("pop-39222");
        }
    }
}
