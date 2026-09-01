using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerController : MonoBehaviour
{
    private static AudioMixerController instance;
    public static AudioMixerController Instance { get => instance; }

    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    /// <summary>
    /// 전체 볼륨 조절
    /// </summary>
    /// <param name="volume"></param>
    public void SetMasterVolume(float volume)
    {   
        // Debug.Log("마스터볼륨 설정: " + volume);
        audioMixer.SetFloat("Master", CalculateAudioMixerVolume(volume));
    }
    
    /// <summary>
    /// 배경음 볼륨 조절
    /// </summary>
    /// <param name="volume"></param>
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", CalculateAudioMixerVolume(volume));
    }
    
    /// <summary>
    /// 효과음 볼륨 조절
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", CalculateAudioMixerVolume(volume));
    }

    private float CalculateAudioMixerVolume(float volume)
    {
        return Mathf.Log10(volume) * 20;
    }
}
