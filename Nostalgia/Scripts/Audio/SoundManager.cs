using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Fusion;
using Nostal.Network;
using UnityEngine.Events;

[Serializable]
public class AudioClipItem      //딕셔너리의 요소인 오디오클립아이템 클래스
{
    [SerializeField] public string name;
    [SerializeField] public AudioClip clip;
}

[Serializable]
public class NewDict        //딕셔너리 클래스
{
    [SerializeField] AudioClipItem[] audioClipItems;    

    public Dictionary<string, AudioClip> ToDictionary() 
    {
        Dictionary<string, AudioClip> newDict = new Dictionary<string, AudioClip>();

        foreach(var item in audioClipItems)
        {
            newDict.Add(item.name, item.clip);
        }

        return newDict;
    }
}

[Serializable]
public class AudioClipTable        //딕셔너리 클래스
{
    [SerializeField] AudioClipItem[] audioClipItems;    

    public Dictionary<string, AudioClip> ToDictionary() 
    {
        Dictionary<string, AudioClip> newDict = new Dictionary<string, AudioClip>();

        foreach(var item in audioClipItems)
        {
            newDict.Add(item.name, item.clip);
        }

        return newDict;
    }
}

public class SoundManager : NetworkBehaviour
{
    private NetworkRunner m_runner => NetworkManager.Instance.Runner;
    
    [Header("Audio Clip Tables")]
    [SerializeField] private AudioClipTable m_sfxClipTable;
    // [SerializeField] private NewDict newSfxDict;      
    // [SerializeField] private NewDict newBgmDict;      
    
    private Dictionary<string, AudioClip> m_sfxDictionary;           
    // private Dictionary<string, AudioClip> m_bgmDictionary;           

    private SoundController soundController;    
    private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;
    
    #region Singleton Pattern
    
    private static SoundManager instance = null;
    
    /// <summary>
    /// SoundManager 싱글톤 구현
    /// </summary>
    public static SoundManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }
    
    #endregion

    public override void Spawned() {
        // Debug.Log("SoundManager Spawned");
        
        audioSource = gameObject.GetComponent<AudioSource>();
        m_sfxDictionary = m_sfxClipTable.ToDictionary();
        // m_bgmDictionary = newBgmDict.ToDictionary();

        // 싱글톤
        if (instance == null)
        {
            instance = this;
            m_runner.MakeDontDestroyOnLoad(this.gameObject);
        }
        else
        {
            m_runner.DestroySingleton<SoundManager>();
            m_runner.Despawn(this.GetComponent<NetworkObject>());
        }
    }

    //효과음 재생
    public void SFX_Play(string clipName, GameObject gameObj = null, float maxDistance = 20)
    {
        if (gameObj == null) soundController = this.GetComponent<SoundController>();  // 사운드 매니져에서 소리 발생
        else soundController = gameObj.GetComponent<SoundController>();                  // 특정 오브젝트에서 소리 발생

        audioClip = m_sfxDictionary[clipName];
        soundController.audioPlay(audioClip, maxDistance);
    }

    public void Set_SFX_LocalDistance(string clipName, GameObject gameObj, float maxDistance){
        soundController = gameObj.GetComponent<SoundController>(); 

        audioClip = m_sfxDictionary[clipName];
        soundController.Set_AudioDistance(audioClip, maxDistance);
    }

    //효과음 네트워크에서 재생
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SFX_Play_rpc(string clipName, NetworkObject gameObj = null, float maxDistance = 20)
    {
        if(gameObj == null) soundController = this.GetComponent<SoundController>(); //사운드 매니져에서 소리 발생 
        else soundController = gameObj.GetComponent<SoundController>();             //특정 오브젝트에서 소리 발생

        audioClip = m_sfxDictionary[clipName];
        soundController.audioPlay(audioClip, maxDistance);
    }
    
    //효과음 네트워크에서 재생
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SFX_Play_dautherOnly_rpc(string clipName, NetworkObject gameObj = null)
    {
        if(gameObj == null) soundController = this.GetComponent<SoundController>(); //사운드 매니져에서 소리 발생 
        else soundController = gameObj.GetComponent<SoundController>();             //특정 오브젝트에서 소리 발생

        audioClip = m_sfxDictionary[clipName];
        soundController.audioPlayDaughterOnly(audioClip);
    }

    //효과음 정지
    public void SFX_Stop(string clipName, GameObject gameObject = null)
    {
        if (gameObject == null) soundController = this.GetComponent<SoundController>();
        else soundController = gameObject.GetComponent<SoundController>();

        soundController.audioStop(clipName);
    }

    //효과음 네트워크에서 정지
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SFX_Stop_rpc(string clipName, NetworkObject gameObject = null)
    {
        if(gameObject == null) soundController = this.GetComponent<SoundController>();  
        else soundController = gameObject.GetComponent<SoundController>();

        soundController.audioStop(clipName);
    }

    //루프 효과음 재생(걷는소리, 일기장 소리 등)
    public void SFX_loop_Play(string clipName, GameObject gameObj = null, float maxDistance = 20){
        if(gameObj == null) soundController = this.GetComponent<SoundController>(); //사운드 매니져에서 소리 발생 
        else soundController = gameObj.GetComponent<SoundController>();             //특정 오브젝트에서 소리 발생

        audioClip = m_sfxDictionary[clipName];
        soundController.loopAudioPlay(audioClip, maxDistance);
    }

    //루프 효과음 네트워크에서 재생(걷는소리, 일기장 소리 등)
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SFX_loop_Play_rpc(string clipName, NetworkObject gameObj = null, float maxDistance = 20){
        if(gameObj == null) soundController = this.GetComponent<SoundController>(); //사운드 매니져에서 소리 발생 
        else soundController = gameObj.GetComponent<SoundController>();             //특정 오브젝트에서 소리 발생

        audioClip = m_sfxDictionary[clipName];
        soundController.loopAudioPlay(audioClip, maxDistance);
    }

    public void SFX_loop_Stop(string clipName, GameObject gameObject = null){
        if(gameObject == null) soundController = this.GetComponent<SoundController>();  
        else soundController = gameObject.GetComponent<SoundController>();

        soundController.loopAudioStop(clipName);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SFX_loop_Stop_rpc(string clipName, NetworkObject gameObject = null){
        if(gameObject == null) soundController = this.GetComponent<SoundController>();  
        else soundController = gameObject.GetComponent<SoundController>();

        soundController.loopAudioStop(clipName);
    }

    public void SFX_Set_Volume(string clipName, GameObject gameObj = null, float volume = 1.0f){
        if(gameObj == null) soundController = this.GetComponent<SoundController>(); //사운드 매니져에서 소리 발생 
        else soundController = gameObj.GetComponent<SoundController>();             //특정 오브젝트에서 소리 발생

        audioClip = m_sfxDictionary[clipName];
        soundController.SetLoopAudioVolume(audioClip, volume);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ResetSoundObjectRpc(NetworkObject gameObj) {
        soundController = gameObj.GetComponent<SoundController>();

        soundController.ResetAudioObject();
    }
}
