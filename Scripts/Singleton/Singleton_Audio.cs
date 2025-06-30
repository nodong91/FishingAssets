using System.Collections;
using UnityEngine;

public class Singleton_Audio : MonoSingleton<Singleton_Audio>
{
    public void Call()
    {
        Debug.LogWarning("SoundManager Call()");
    }
    [SerializeField]
    private AudioSource bgmSource01, bgmSource02, fxSource;
    public AudioSource bgmSource;
    public bool bgmSet = false;

    public bool BGMMute;
    public float BGMVolume;
    public bool fxMute;
    public float fxVolume;

    public void SetAudio()
    {
        bgmSource01 = gameObject.AddComponent<AudioSource>();
        bgmSource01.loop = true;
        bgmSource01.mute = BGMMute;
        bgmSource01.volume = BGMVolume;
        bgmSource01.playOnAwake = false;

        bgmSource02 = gameObject.AddComponent<AudioSource>();
        bgmSource02.loop = true;
        bgmSource02.mute = true;
        bgmSource02.volume = BGMVolume;
        bgmSource02.playOnAwake = false;

        fxSource = gameObject.AddComponent<AudioSource>();
        fxSource.mute = fxMute;
        fxSource.volume = fxVolume;
    }

    public void Audio_SetBGM(string _id)
    {
        bgmSet = !bgmSet;
        //  추가 음악 변경
        AudioSource newSource = (bgmSet == true) ? bgmSource01 : bgmSource02;
        newSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        newSource.loop = true;
        newSource.Play();

        if (changeBGM != null)
            StopCoroutine(changeBGM);
        changeBGM = StartCoroutine(CrossFadeAudio(newSource));
    }
    Coroutine changeBGM;
    public void SetBGMVolume(float _value)
    {
        BGMVolume = _value;
        bgmSource01.volume = BGMVolume;
    }

    IEnumerator CrossFadeAudio(AudioSource _newSource)
    {
        _newSource.mute = BGMMute;
        if (bgmSource != null)
        {
            float normalize = 0.0f;
            while (normalize < 1.0f)
            {
                normalize += Time.fixedDeltaTime * 0.5f;
                _newSource.volume = Mathf.Lerp(0.0f, BGMVolume, normalize);
                bgmSource.volume = BGMVolume - _newSource.volume;
                yield return null;
            }
            bgmSource.mute = true;
        }
        bgmSource = _newSource;
    }

    public void Audio_SetFX(string _id)
    {
        fxSource.Stop();
        fxSource.pitch = Random.Range(0.7f, 1.3f);
        fxSource.clip = Singleton_Data.INSTANCE.Dict_Audio[_id];
        fxSource.Play();
    }

    public void SetFXVolume(float _value)
    {
        fxVolume = _value;
        fxSource.volume = fxVolume;
    }
}
