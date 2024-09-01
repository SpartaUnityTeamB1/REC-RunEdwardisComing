using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    private AudioMixer audioMixer;
    private List<AudioMixerGroup> audioMixerGroups;

    private AudioSource[] bgmSources = new AudioSource[2];
    private Queue<AudioSource> sfxSources = new Queue<AudioSource>();

    [Range(0.0001f, 1f)] private List<float> groupVolumes = new List<float>();

    private GameObject obj = null;
    private AudioSource source;

    private Coroutine changeCor = null;

    private float current;
    private float lerpTime = 1f;

    private int queueCount;

    public void InitSoundMgr(AudioMixer audioMixer, List<AudioMixerGroup> audioMixerGroups, int queueCount)
    {
        this.audioMixer = audioMixer;
        this.audioMixerGroups = audioMixerGroups;

        this.queueCount = queueCount;

        MakeObject();
        SettingSources();

        for (int i = 0; i < audioMixerGroups.Count; ++i)
            groupVolumes.Add(PlayerPrefs.GetFloat(((MixerGroupType)i).ToString(), 1f));
    }

    private void MakeObject()
    {
        if (null == obj)
        {
            obj = new GameObject(GetType().Name);

            GameManager.Instance.DontDestroy(obj);
        }
    }

    private void SettingSources()
    {
        for (int i = 0; i < bgmSources.Length; ++i)
        {
            bgmSources[i] = obj.AddComponent<AudioSource>();

            bgmSources[i].loop = true;
            bgmSources[i].outputAudioMixerGroup = audioMixerGroups[(int)MixerGroupType.BGM];
        }

        for (int i = 0; i< queueCount; ++i)
        {
            source = obj.AddComponent<AudioSource>();
            
            source.loop = false;
            source.outputAudioMixerGroup = audioMixerGroups[(int)MixerGroupType.SFX];

            sfxSources.Enqueue(source);
        }
    }

    public void PlayBGM (AudioClip bgmClip)
    {
        if (null != changeCor)
        {
            GameManager.Instance.CoroutineStop(changeCor);
            changeCor = null;
        }

        if (!bgmSources[0].isPlaying)
        {
            bgmSources[0].clip = bgmClip;
            changeCor = GameManager.Instance.CoroutineStart(ChangeBGMClip(bgmSources[0], bgmSources[1]));
        }
        else
        {
            bgmSources[1].clip = bgmClip;
            changeCor = GameManager.Instance.CoroutineStart(ChangeBGMClip(bgmSources[0], bgmSources[1]));
        }
    }

    private IEnumerator ChangeBGMClip(AudioSource target, AudioSource turnOff)
    {
        current = 0f;

        target.Play();

        while (current < lerpTime)
        {
            current += Time.deltaTime;

            target.volume = Mathf.Lerp(0f, 1f, (current / lerpTime));
            turnOff.volume = Mathf.Lerp(1f, 0f, (current / lerpTime));

            yield return null;
        }

        target.volume = 1f;
        turnOff.Stop();
        turnOff.clip = null;

        changeCor = null;
    }

    public void StopBGM()
    {
        foreach (var source in bgmSources)
            source.Stop();
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        foreach (var sfxSource in sfxSources)
        {
            if (!sfxSource.isPlaying)
            {
                sfxSource.PlayOneShot(sfxClip);
                return;
            }    
        }

        source = sfxSources.Dequeue();
        source.Stop();

        source.PlayOneShot(sfxClip);

        sfxSources.Enqueue(source);
    }

    public void SetBGMVolume(float volume)
    {
        groupVolumes[(int)MixerGroupType.BGM] = volume;

        SettingVolumes();

        PlayerPrefs.SetFloat((MixerGroupType.BGM).ToString(), groupVolumes[(int)MixerGroupType.BGM]);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        groupVolumes[(int)MixerGroupType.SFX] = volume;

        SettingVolumes();

        PlayerPrefs.SetFloat((MixerGroupType.SFX).ToString(), groupVolumes[(int)MixerGroupType.SFX]);
        PlayerPrefs.Save();
    }

    public float GetBGMVolume()
    {
        return groupVolumes[(int)MixerGroupType.BGM];
    }

    public float GetSFXVolume()
    {
        return groupVolumes[(int)MixerGroupType.SFX];
    }

    public void SettingVolumes()
    {
        for (int i = 0; i < groupVolumes.Count; ++i)
            audioMixer.SetFloat(((MixerGroupType)i).ToString(), Mathf.Log10(groupVolumes[i]) * 20f);
    }
}
