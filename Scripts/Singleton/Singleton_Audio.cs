using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton_Audio : MonoSingleton<Singleton_Audio>
{
    public bool masterMute;
    public float masterVolume;

    public AudioSource BGMSource;
    public bool bgmMute;
    public float bgmVolume;

    public AudioSource fxSource;
    public bool fxMute;
    public float fxVolume;

    public AudioSource envSource;
    public bool envMute;
    public float envVolume;

    Queue<AudioSource> audioQueue = new Queue<AudioSource>();

    AudioSource TryAudioSource()
    {
        if (audioQueue.Count > 0)
            return audioQueue.Dequeue();

        AudioSource audioSource = new GameObject("[ InstanteAudio ]").AddComponent<AudioSource>();
        audioSource.transform.SetParent(transform, false);
        return audioSource;
    }

    //===========================================================================================================================
    // 배경 음악
    //===========================================================================================================================

    public void Audio_BGM(string _id)
    {
        AudioSource audioSource = (_id != null) ? TryAudioSource() : null;
        if (_id != null)
        {
            audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
            audioSource.mute = masterMute == true ? true : bgmMute;
            audioSource.volume = bgmVolume * masterVolume;
            audioSource.loop = true;
            audioSource.pitch = 1.0f;
            audioSource.Play();
        }
        StartCoroutine(PlayBGMAudio(audioSource));
    }

    public void SetMasterVolume(float _value)
    {
        masterVolume = _value;
        if (BGMSource != null)
            BGMSource.volume = bgmVolume * _value;
        if (fxSource != null)
            fxSource.volume = fxVolume * _value;
        if (envSource != null)
            envSource.volume = envVolume * _value;
    }

    public void SetMasterMute(bool _isOn)
    {
        masterMute = _isOn;
        if (BGMSource != null)
            BGMSource.mute = masterMute == true ? true : bgmMute;
        if (fxSource != null)
            fxSource.mute = masterMute == true ? true : fxMute;
        if (envSource != null)
            envSource.mute = masterMute == true ? true : envMute;
    }

    public void SetBGMVolume(float _value)
    {
        bgmVolume = _value;
        if (BGMSource != null)
            BGMSource.volume = _value * masterVolume;
    }

    public void SetBGMMute(bool _isOn)
    {
        bgmMute = _isOn;
        if (BGMSource != null)
            BGMSource.mute = masterMute == true ? true : bgmMute;
    }

    IEnumerator PlayBGMAudio(AudioSource _audioSource)
    {
        if (BGMSource != null)
        {
            AudioSource originSource = BGMSource;
            float normalize = 0.0f;
            while (normalize < 1.0f)
            {
                normalize += Time.fixedDeltaTime * 0.5f;
                float volume = Mathf.Lerp(0.0f, bgmVolume, normalize) * masterVolume;
                if (_audioSource != null)
                    _audioSource.volume = volume;
                originSource.volume = bgmVolume - volume;
                yield return null;
            }
            originSource.volume = 0.0f;
            originSource.Stop();
            audioQueue.Enqueue(originSource);
        }
        BGMSource = _audioSource;
    }

    //===========================================================================================================================
    // 효과음
    //===========================================================================================================================

    public void Audio_FX(string _id)
    {
        if (_id == null)
            return;

        AudioSource audioSource = TryAudioSource();
        Debug.Log($"{_id} : {audioSource}");
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        audioSource.mute = masterMute == true ? true : fxMute;
        audioSource.volume = fxVolume * masterVolume;
        audioSource.loop = false;
        audioSource.pitch = 1f;
        audioSource.Play();

        fxSource = audioSource;
        StartCoroutine(PlayFXAudio(audioSource));
    }

    public void Audio_Dialog(string _id)
    {
        if (_id == null)
            return;

        AudioSource audioSource = TryAudioSource();
        Debug.Log($"{_id} : {audioSource}");
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        audioSource.mute = masterMute == true ? true : fxMute;
        audioSource.volume = fxVolume * masterVolume;
        audioSource.loop = false;
        audioSource.pitch = Random.Range(0.7f, 1.3f);
        audioSource.Play();

        fxSource = audioSource;
        StartCoroutine(PlayFXAudio(audioSource));
    }

    IEnumerator PlayFXAudio(AudioSource _audioSource)
    {
        float clipLength = _audioSource.clip.length;
        yield return new WaitForSeconds(clipLength);

        audioQueue.Enqueue(_audioSource);
    }

    public void SetFXMute(bool _isOn)
    {
        fxMute = _isOn;
        if (fxSource != null)
            fxSource.mute = masterMute == true ? true : fxMute;
    }

    public void SetFXVolume(float _value)
    {
        fxVolume = _value;
        if (fxSource != null)
            fxSource.volume = _value * masterVolume;
    }

    //===========================================================================================================================
    // 환경음
    //===========================================================================================================================

    public void Audio_Environment(string _id)
    {
        ResetEnvironment();
        if (_id == null)
            return;

        AudioSource audioSource = TryAudioSource();
        Debug.LogWarning($"{_id} : {audioSource}");
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        audioSource.mute = masterMute == true ? true : envMute;
        audioSource.volume = envVolume * masterVolume;
        audioSource.loop = true;
        audioSource.pitch = 1f;
        audioSource.Play();

        envSource = audioSource;
    }

    public void SetEnvironmentMute(bool _isOn)
    {
        envMute = _isOn;
        if (envSource != null)
            envSource.mute = masterMute == true ? true : envMute;
    }

    public void SetEnvironmentVolume(float _value)
    {
        envVolume = _value;
        if (envSource != null)
            envSource.volume = _value * masterVolume;
    }

    //===========================================================================================================================
    // 리셋
    //===========================================================================================================================

    void ResetEnvironment()
    {
        StartCoroutine(PlayEnvironmentAudio());
    }

    IEnumerator PlayEnvironmentAudio()
    {
        if (envSource != null)
        {
            float normalize = 0.0f;
            while (normalize < 1.0f)
            {
                normalize += Time.fixedDeltaTime * 0.5f;
                float volume = Mathf.Lerp(0.0f, bgmVolume, normalize) * masterVolume;
                envSource.volume = bgmVolume - volume;
                yield return null;
            }
            envSource.Stop();
            audioQueue.Enqueue(envSource);
        }
    }
}
