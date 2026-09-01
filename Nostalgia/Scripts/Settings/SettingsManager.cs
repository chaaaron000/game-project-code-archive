using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Steamworks;
using Unity.Services.Vivox;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour
{
    #region Singleton Pattern

    private static SettingsManager instance = null;

    /// <summary>
    /// SettingsManager 싱글톤 구현
    /// </summary>
    public static SettingsManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    #endregion

    private string _settingsFilePath;
    
    public GameSettings currentGameSettings;
    
    private void Awake()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnEnable()
    {
        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _settingsFilePath = Path.Combine(Application.persistentDataPath, "gamesettings.json");
        LoadSettings();
        ApplySettings();
    }
    
    // 세팅 불러오기
    public void LoadSettings()
    {
        // 파일이 있는지 확인
        if (File.Exists(_settingsFilePath))
        {
            string json = File.ReadAllText(_settingsFilePath);
            currentGameSettings = JsonUtility.FromJson<GameSettings>(json);
        }
        // 없으면 기본 게임 세팅으로
        else
        {
            currentGameSettings = new GameSettings();
            SaveSettings();
        }
    }
    
    // 세팅을 저장
    public void SaveSettings()
    {
        if (currentGameSettings == null) return;

        Debug.Log("-------In SaveSettings---------");
        currentGameSettings.ShowSettings();

        string json = JsonUtility.ToJson(currentGameSettings, true);
        File.WriteAllText(_settingsFilePath, json);
    }

    // TODO: 추가되는 세팅 기능들은 여기서 연결할 것. 밝기의 경우 Controller의 slider callback에도 연결해야함
    public void ApplySettings()
    {
        Debug.Log("-------In ApplySettings---------");
        currentGameSettings.ShowSettings();
        ChangeResolutionSettings(
            currentGameSettings.resolutionWidth, 
            currentGameSettings.resolutionHeight,
            currentGameSettings.displayMode,
            currentGameSettings.refreshRate
        );

        AudioMixerController.Instance.SetMasterVolume(currentGameSettings.masterVolume);
        AudioMixerController.Instance.SetBGMVolume(currentGameSettings.bgmVolume);
        AudioMixerController.Instance.SetSFXVolume(currentGameSettings.sfxVolume);

        // 밝기 조절
        if (GameObject.Find("Global Volume") != null && GameObject.Find("Global Volume").TryGetComponent(out GlobalVolumeController gvc))
            gvc.ChangeGammaOffsetValue(currentGameSettings.brightness);
        else
            Debug.LogWarning("GlobalVolumeController를 찾을 수 없습니다");
        
        VivoxManager.Instance.ChangeOutputDevice(currentGameSettings.voiceChatOutputDevice);
        VivoxManager.Instance.ChangeInputDevice(currentGameSettings.microphoneDevice);
        VivoxManager.Instance.SetOutputDeviceVolume(currentGameSettings.voiceChatVolume);
        VivoxManager.Instance.SetInputDeviceVolume(currentGameSettings.microphoneVolume);
    }

    void ChangeResolutionSettings(int width, int height, int modeIndex, int refreshRate)
    {
        FullScreenMode mode = (FullScreenMode)Mathf.Clamp(modeIndex, 0, 3);
        RefreshRate rr = new RefreshRate() { numerator = Convert.ToUInt32(refreshRate), denominator = 1 };
        Screen.SetResolution(width, height, mode, rr);

        Debug.Log($"ChangeResolutionSettings : {width} x {height} @ {refreshRate} Hz, {mode}");
    }
    
    /// <summary>
    /// 씬이 로드될 때마다 세팅을 적용하도록 함
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SettingsManager OnSceneLoaded");
        _settingsFilePath = Path.Combine(Application.persistentDataPath, "gamesettings.json");
        LoadSettings();
        
        ApplySettings();
    }
}
