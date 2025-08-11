using System;
using System.Collections.Generic;
using System.Reflection;
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
    public Slider bgm_Slider, fx_Slider, env_Slider;
    public Toggle bgm_Mute, fx_Mute, env_Mute;
    const float divide = 10f;
    bool onSet;
    Dictionary<string, int> tryStringToIndex = new Dictionary<string, int>();
    //===========================================================================================================================
    public void SetStart()
    {
        tryStringToIndex.Clear();
        for (int i = 0; i < audioStrings.Length; i++)
        {
            tryStringToIndex[audioStrings[i]] = i;
        }
        prevButton.onClick.AddListener(delegate { NextButton(-1); });
        nextButton.onClick.AddListener(delegate { NextButton(1); });

        bgm_Mute.onValueChanged.AddListener(BGMMute);
        bgm_Slider.onValueChanged.AddListener(BGMVolume);
        bgm_Slider.maxValue = divide;
        bgm_Slider.wholeNumbers = true;

        fx_Mute.onValueChanged.AddListener(FXMute);
        fx_Slider.onValueChanged.AddListener(FXVolume);
        fx_Slider.maxValue = divide;
        fx_Slider.wholeNumbers = true;

        env_Mute.onValueChanged.AddListener(EnvMute);
        env_Slider.onValueChanged.AddListener(EnvVolume);
        env_Slider.maxValue = divide;
        env_Slider.wholeNumbers = true;

        SetAudioUI();
        onSet = true;
    }

    void SetAudioUI()
    {
        AudioStruct audioStruct = Option_Manager.current.optionData.audioStruct;
        // UI ¼¼ÆÃ
        bgm_Slider.value = audioStruct.bgmVolume * divide;
        bgm_Mute.isOn = audioStruct.bgmMute;
        fx_Slider.value = audioStruct.fxVolume * divide;
        fx_Mute.isOn = audioStruct.fxMute;
        env_Slider.value = audioStruct.envVolume * divide;
        env_Mute.isOn = audioStruct.envMute;
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
        currentAudio = _index;
        audioText.text = audioStrings[_index];
        Singleton_Audio.INSTANCE.Audio_BGM(audioStrings[_index]);
    }

    public void PlayBGMAudio(string _id)
    {
        if (tryStringToIndex.TryGetValue(_id, out int index))
        {
            PlayBGMAudio(index);
        }
        else
        {
            Singleton_Audio.INSTANCE.Audio_BGM(null);
            Debug.LogError($"Audio string '{_id}' not found in the list.");
        }
    }

    void BGMVolume(float _value)
    {
        float value = _value / divide;
        Singleton_Audio.INSTANCE.SetBGMVolume(value);
    }

    void BGMMute(bool _isOn)
    {
        Singleton_Audio.INSTANCE.SetBGMMute(_isOn);
    }

    void FXVolume(float _value)
    {
        if (onSet == true)
            SetFxPrev();
        float value = _value / divide;
        Singleton_Audio.INSTANCE.SetFXVolume(value);
    }

    void SetFxPrev()
    {
        string soundName = "pop-39222";
        Singleton_Audio.INSTANCE.Audio_FX(soundName);
    }

    void FXMute(bool _isOn)
    {
        Singleton_Audio.INSTANCE.SetFXMute(_isOn);
    }

    void EnvVolume(float _value)
    {
        float value = _value / divide;
        Singleton_Audio.INSTANCE.SetEnvironmentVolume(value);
    }

    void EnvMute(bool _isOn)
    {
        Singleton_Audio.INSTANCE.SetEnvironmentMute(_isOn);
    }
}
