using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Singleton_Audio : MonoSingleton<Singleton_Audio>
{
    public bool masterMute;
    public float masterVolume;

    public AudioSource BGMSource;
    public bool bgmMute;
    public float bgmVolume;

    public AudioSource fxSource;
    public AudioSource loopFxSource;
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

    //===========================================================================================================================
    // 배경 음악
    //===========================================================================================================================

    public void Audio_BGM(string _id)
    {
        AudioSource audioSource = (_id != null) ? TryAudioSource() : null;
        if (_id != null)
        {
            audioSource.gameObject.SetActive(true);
            audioSource.name = _id;
            Debug.Log($"Audio_BGM : {audioSource.name}");
            audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id].clip;
            audioSource.mute = masterMute == true ? true : bgmMute;
            audioSource.volume = bgmVolume * masterVolume;
            audioSource.loop = true;
            audioSource.pitch = 1.0f;
            audioSource.Play();
        }
        AudioSource origin = BGMSource;
        BGMSource = audioSource;
        StartCoroutine(PlayBGMAudio(origin));
    }

    IEnumerator PlayBGMAudio(AudioSource _origin)
    {
        float targetVolume = bgmVolume * masterVolume;
        float normalize = 0.0f;
        while (normalize < 1.0f)
        {
            normalize += Time.fixedDeltaTime * 2f;
            float volume = Mathf.Lerp(0.0f, targetVolume, normalize);
            if (BGMSource != null)
                BGMSource.volume = volume;
            if (_origin != null)
                _origin.volume = targetVolume - volume;
            yield return null;
        }
        if (_origin == null)
            yield break;
        _origin.volume = 0.0f;
        _origin.Stop();
        _origin.gameObject.SetActive(false);
        audioQueue.Enqueue(_origin);
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

    //===========================================================================================================================
    // 효과음
    //===========================================================================================================================

    public void Audio_FX(string _id)
    {
        if (_id == null)
            return;

        AudioSource audioSource = TryAudioSource();
        audioSource.gameObject.SetActive(true);
        audioSource.name = _id;
        Debug.Log($"효과음 아이디 {_id}");
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id].clip;
        audioSource.mute = masterMute == true ? true : fxMute;
        audioSource.volume = fxVolume * masterVolume;
        audioSource.loop = false;
        audioSource.pitch = 1f;
        audioSource.Play();

        fxSource = audioSource;
        StartCoroutine(PlayFXAudio(audioSource));
    }

    public void Audio_LoopFX(string _id)
    {
        if (_id == null)
            return;

        AudioSource audioSource = TryAudioSource();
        audioSource.gameObject.SetActive(true);
        audioSource.name = _id;
        Debug.Log($"{audioSource.name}");
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id].clip;
        audioSource.mute = masterMute == true ? true : fxMute;
        audioSource.volume = fxVolume * masterVolume;
        audioSource.loop = true;
        audioSource.pitch = 1f;
        audioSource.Play();

        loopFxSource = audioSource;
    }

    public void Stop_LoopFX()
    {
        if (loopFxSource != null)
        {
            loopFxSource.Stop();
            loopFxSource.loop = false;
            loopFxSource.gameObject.SetActive(false);
            audioQueue.Enqueue(loopFxSource);
        }
    }

    public void Audio_Dialog(string _id)
    {
        if (_id == null)
            return;

        AudioSource audioSource = TryAudioSource();
        audioSource.gameObject.SetActive(true);
        audioSource.name = _id;
        Debug.Log($"{audioSource}");
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id].clip;
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

        _audioSource.gameObject.SetActive(false);
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
        if (envSource != null)// 기존 환경음 정지
        {
            envSource.Stop();
            envSource.gameObject.SetActive(false);
            audioQueue.Enqueue(envSource);
        }

        if (_id == null)
            return;

        AudioSource audioSource = TryAudioSource();
        audioSource.gameObject.SetActive(true);
        audioSource.name = _id;
        //Debug.LogWarning($"배경음 : {_id}");
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id].clip;
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

    IEnumerator PlayEnvironmentAudio()
    {
        if (envSource != null)
        {
            float targetVolume = envVolume * masterVolume;
            float normalize = 0.0f;
            while (normalize < 1.0f)
            {
                normalize += Time.fixedDeltaTime * 0.5f;
                float volume = Mathf.Lerp(0.0f, targetVolume, normalize);
                envSource.volume = targetVolume - volume;
                yield return null;
            }
            envSource.Stop();
            envSource.gameObject.SetActive(false);
            audioQueue.Enqueue(envSource);
        }
    }
}
