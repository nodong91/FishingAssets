using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton_Audio : MonoSingleton<Singleton_Audio>
{
    private AudioSource BGMSource;
    public bool bgmMute;
    public float bgmVolume;

    private AudioSource fxSource;
    public bool fxMute;
    public float fxVolume;

    Coroutine changeBGM;
    Queue<AudioSource> audioQueue = new Queue<AudioSource>();

    AudioSource TryAudioSource()
    {
        if (audioQueue.Count > 0)
            return audioQueue.Dequeue();

        AudioSource audioSource = new GameObject("[ InstanteAudio ]").AddComponent<AudioSource>();
        audioSource.transform.SetParent(transform, false);
        return audioSource;
    }

    public void Audio_BGM(string _id)
    {
        AudioSource audioSource = TryAudioSource();
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        audioSource.loop = true;
        audioSource.mute = bgmMute;
        audioSource.volume = bgmVolume;
        audioSource.Play();

        if (changeBGM != null)
            StopCoroutine(changeBGM);
        changeBGM = StartCoroutine(PlayBGMAudio(audioSource));
    }

    public void SetBGMVolume(float _value)
    {
        bgmVolume = _value;
        if (BGMSource != null)
            BGMSource.volume = bgmVolume;
    }

    public void SetBGMMute(bool _isOn)
    {
        bgmMute = _isOn;
        if (BGMSource != null)
            BGMSource.mute = _isOn;
    }

    IEnumerator PlayBGMAudio(AudioSource _audioSource)
    {
        _audioSource.mute = bgmMute;
        if (BGMSource != null)
        {
            float normalize = 0.0f;
            while (normalize < 1.0f)
            {
                normalize += Time.fixedDeltaTime * 0.5f;
                _audioSource.volume = Mathf.Lerp(0.0f, bgmVolume, normalize);
                BGMSource.volume = bgmVolume - _audioSource.volume;
                yield return null;
            }
            BGMSource.Stop();
            audioQueue.Enqueue(BGMSource);
        }
        BGMSource = _audioSource;
    }

    public void Audio_FX(string _id)
    {
        AudioSource audioSource = TryAudioSource();
        audioSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        audioSource.loop = false;
        audioSource.mute = fxMute;
        audioSource.volume = fxVolume;
        audioSource.pitch = Random.Range(0.7f, 1.3f);
        audioSource.Play();

        StartCoroutine(PlayFXAudio(audioSource));
    }

    IEnumerator PlayFXAudio(AudioSource _audioSource)
    {
        float clipLength = _audioSource.clip.length;
        yield return new WaitForSeconds(clipLength);

        audioQueue.Enqueue(_audioSource);
    }

    public void Audio_SetFX(string _id)
    {
        fxSource.Stop();
        fxSource.pitch = Random.Range(0.7f, 1.3f);
        fxSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        fxSource.Play();
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
            fxSource.volume = fxVolume;
    }
}
