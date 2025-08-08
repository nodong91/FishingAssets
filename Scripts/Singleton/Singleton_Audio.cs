using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Singleton_Audio : MonoSingleton<Singleton_Audio>
{
    public AudioSource BGMSource;
    public bool bgmMute;
    public float bgmVolume;

    public AudioSource fxSource;
    public bool fxMute;
    public float fxVolume;

    public AudioSource envSource;
    public bool envMute;
    public float envVolume;

    Coroutine changeBGM;
    Queue<AudioSource> audioQueue = new Queue<AudioSource>();

    AudioSource TryAudioSource()
    {
        Debug.LogWarning($"빈거 있는지 : {audioQueue.Count > 0}");
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
        Debug.LogWarning($"{_id} : {audioSource}");
        if (_id != null)
        {
            audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
            audioSource.mute = bgmMute;
            audioSource.volume = bgmVolume;
            audioSource.loop = true;
            audioSource.pitch = 1.0f;
            audioSource.Play();
        }

        if (changeBGM != null)
            StopCoroutine(changeBGM);
        changeBGM = StartCoroutine(PlayBGMAudio(audioSource));
    }

    public void SetBGMVolume(float _value)
    {
        bgmVolume = _value;
        if (BGMSource != null)
            BGMSource.volume = _value;
    }

    public void SetBGMMute(bool _isOn)
    {
        bgmMute = _isOn;
        if (BGMSource != null)
            BGMSource.mute = _isOn;
    }

    IEnumerator PlayBGMAudio(AudioSource _audioSource)
    {
        if (BGMSource != null)
        {
            float normalize = 0.0f;
            while (normalize < 1.0f)
            {
                normalize += Time.fixedDeltaTime * 0.5f;
                float volume = Mathf.Lerp(0.0f, bgmVolume, normalize);
                if (_audioSource != null)
                    _audioSource.volume = volume;
                BGMSource.volume = bgmVolume - volume;
                yield return null;
            }
            BGMSource.Stop();
            audioQueue.Enqueue(BGMSource);
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
        Debug.LogWarning($"{_id} : {audioSource}");
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        audioSource.mute = fxMute;
        audioSource.volume = fxVolume;
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
        Debug.LogWarning($"{_id} : {audioSource}");
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        audioSource.mute = fxMute;
        audioSource.volume = fxVolume;
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
            fxSource.mute = _isOn;
    }

    public void SetFXVolume(float _value)
    {
        fxVolume = _value;
        if (fxSource != null)
            fxSource.volume = _value;
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
        audioSource.mute = envMute;
        audioSource.volume = envVolume;
        audioSource.loop = true;
        audioSource.pitch = 1f;
        audioSource.Play();

        envSource = audioSource;
    }

    public void SetEnvironmentMute(bool _isOn)
    {
        envMute = _isOn;
        if (envSource != null)
            envSource.mute = _isOn;
    }

    public void SetEnvironmentVolume(float _value)
    {
        envVolume = _value;
        if (envSource != null)
            envSource.volume = _value;
    }







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
                float volume = Mathf.Lerp(0.0f, bgmVolume, normalize);
                envSource.volume = bgmVolume - volume;
                yield return null;
            }
            envSource.Stop();
            audioQueue.Enqueue(envSource);
        }
    }
}
