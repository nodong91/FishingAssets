using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Data_Manager.Data_Option;

public class Option_Audio : MonoBehaviour
{
    public Custom_Button prevButton, nextButton;
    public TMP_Text audioText;
    public int currentAudio;
    //public string[] audioStrings;
    public Slider master_Slider, bgm_Slider, fx_Slider, env_Slider;
    public Toggle master_Mute, bgm_Mute, fx_Mute, env_Mute;
    const float divide = 10f;
    bool onSet;
    Dictionary<Data_Manager.AudioStruct, int> tryAudioStructToIndex = new Dictionary<Data_Manager.AudioStruct, int>();

    //===========================================================================================================================
    public void SetStart()
    {
        tryAudioStructToIndex.Clear();
        int index = 0;
        foreach (var child in Singleton_Data.INSTANCE.Dict_Audio)
        {
            if(child.Value.type == Data_Manager.AudioStruct.AudioType.BGM)
            {
                index++;
                tryAudioStructToIndex[child.Value] = index;
            }
        }
        
        prevButton.SetButton(delegate { NextButton(-1); }, OnArrowButton, OffArrowButton);
        nextButton.SetButton(delegate { NextButton(1); }, OnArrowButton, OffArrowButton);

        master_Mute.onValueChanged.AddListener(MasterMute);
        master_Slider.onValueChanged.AddListener(MasterVolume);
        master_Slider.maxValue = divide;
        master_Slider.wholeNumbers = true;

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
        // UI 세팅
        MasterVolume(audioStruct.masterVolume * divide);
        MasterMute(audioStruct.masterMute);

        BGMVolume(audioStruct.bgmVolume * divide);
        BGMMute(audioStruct.bgmMute);

        FXVolume(audioStruct.fxVolume * divide);
        FXMute(audioStruct.fxMute);

        EnvVolume(audioStruct.envVolume * divide);
        EnvMute(audioStruct.envMute);
    }

    void NextButton(int _index)
    {
        int index = currentAudio + _index;
        if (index >= tryAudioStructToIndex.Count)
        {
            index = 0;
        }
        else if (index < 0)
        {
            index = tryAudioStructToIndex.Count - 1;
        }
        PlayBGMAudio(index);
    }

    void PlayBGMAudio(int _index)
    {
        currentAudio = _index;
        if (TryAudioStruct(_index, out Data_Manager.AudioStruct _data) == false)
        {
            return;
        }
        audioText.text = _data.clip.name;
        Singleton_Audio.INSTANCE.Audio_BGM(_data.id);
    }
    bool TryAudioStruct(int _index, out Data_Manager.AudioStruct _data)
    {
        foreach (var child in tryAudioStructToIndex)
        {
            if (child.Value.Equals(_index))
            {
                _data = child.Key;
                return true;
            }
        }
        _data = default;
        return false;
    }


    public void PlayBGMAudio(string _id)
    {
        Data_Manager.AudioStruct data = Singleton_Data.INSTANCE.Dict_Audio[_id];
        if (tryAudioStructToIndex.TryGetValue(data, out int index))// 아이템 찾기
        {
            PlayBGMAudio(index);
        }
        else
        {
            Singleton_Audio.INSTANCE.Audio_BGM(null);
            Debug.LogError($"Audio string '{_id}' not found in the list.");
        }
    }

    void MasterVolume(float _value)
    {
        master_Slider.value = _value;
        float value = _value / divide;
        Singleton_Audio.INSTANCE.SetMasterVolume(value);
    }

    void MasterMute(bool _isOn)
    {
        master_Mute.isOn = _isOn;
        Singleton_Audio.INSTANCE.SetMasterMute(_isOn);
    }

    void BGMVolume(float _value)
    {
        bgm_Slider.value = _value;
        float value = _value / divide;
        Singleton_Audio.INSTANCE.SetBGMVolume(value);
    }

    void BGMMute(bool _isOn)
    {
        bgm_Mute.isOn = _isOn;
        Singleton_Audio.INSTANCE.SetBGMMute(_isOn);
    }

    void FXVolume(float _value)
    {
        if (onSet == true)
            SetFxPrev();
        fx_Slider.value = _value;
        float value = _value / divide;
        Singleton_Audio.INSTANCE.SetFXVolume(value);
    }

    void FXMute(bool _isOn)
    {
        fx_Mute.isOn = _isOn;
        Singleton_Audio.INSTANCE.SetFXMute(_isOn);
    }

    void EnvVolume(float _value)
    {
        env_Slider.value = _value;
        float value = _value / divide;
        Singleton_Audio.INSTANCE.SetEnvironmentVolume(value);
    }

    void EnvMute(bool _isOn)
    {
        env_Mute.isOn = _isOn;
        Singleton_Audio.INSTANCE.SetEnvironmentMute(_isOn);
    }

    void SetFxPrev()
    {
        Singleton_Audio.INSTANCE.Audio_FX(Const_Audio._buttonClick);
    }

    public Sprite onArrow, offArrow;

    void OnArrowButton(Custom_Button _button)
    {
        _button.GetButtonImage.sprite = onArrow;
        _button.GetButtonImage.CrossFadeAlpha(1f, 0.1f, false);
    }

    void OffArrowButton(Custom_Button _button)
    {
        _button.GetButtonImage.sprite = offArrow;
        _button.GetButtonImage.CrossFadeAlpha(0.5f, 0.1f, false);
    }
}
