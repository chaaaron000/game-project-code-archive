using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Nostal.Util;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;


public class SettingUIController : MonoBehaviour, UIController
{
    [Header("Canvas")] 
    [SerializeField] private Canvas canvas;
    
    [Header("Penels")]
    public GameObject graphicTabPanel;
    public GameObject soundTabPanel;
    public GameObject gamePlayTabPanel;
    public GameObject controlTabPanel;

    [Header("Video Settings")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown displayModeDropdown;
    public TMP_Dropdown refreshRateDropdown;
    public Slider brightnessSlider;

    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public TMP_Dropdown voiceChatOutputDeviceDropdown;
    public Slider voiceChatVolumeSlider;
    public TMP_Dropdown microphoneDeviceDropdown;
    public Slider microphoneVolumeSlider;

    [Header("Control Settings")]
    public Slider mouseSensitivitySlider;

    [Header("Buttons")] 
    public Button graphicTabButton;
    public Button soundTabButton;
    public Button gamePlayTabButton;
    public Button controlTabButton;
    public Button confirmButton;
    public Button cancelButton;
    // public Button applyButton;
    public Button resetButton;

    [Header("Texts")]
    public TMP_Text graphicTabText;
    public TMP_Text soundTabText;
    public TMP_Text gamePlayTabText;
    public TMP_Text controlTabText;

    
    public void Show()
    {
        Debug.Log("SettingUIController : Show()");
        canvas.enabled = true;
        if (SettingsManager.Instance.currentGameSettings != null)
            LoadGameSettingsValue(SettingsManager.Instance.currentGameSettings);
        
        CursorController.SetEnableCursor(true);
        if (Camera.main.TryGetComponent(out FirstPersonCamera fpc))
        {
            fpc.LockCameraRotate(true);
            
        }
    }

    public void Hide()
    {
        Debug.Log("SettingUIController : Hide()");
        canvas.enabled = false;
        
        CursorController.SetEnableCursor(false);
        if (Camera.main.TryGetComponent(out FirstPersonCamera fpc))
        {
            fpc.LockCameraRotate(false);
        }
    }

    private void OnEnable()
    {
        Setup();
    }

    // private void Start()
    // {
    //     Show();
    // }

    private void OnClickGraphicTabButton(){
        AllPanelSetActiveFalse();
        if(!graphicTabPanel.activeSelf){
            graphicTabPanel.SetActive(true);
        }
        SetHighlightedColor(graphicTabButton, 0f);
        graphicTabText.color = Color.black;
    }

    private void OnClickSoundTabButton(){
        AllPanelSetActiveFalse();
        if(!soundTabPanel.activeSelf){
            soundTabPanel.SetActive(true);
        }
        SetHighlightedColor(soundTabButton, 0f);
        soundTabText.color = Color.black;
    }

    private void OnClickGamePlayTabButton(){
        AllPanelSetActiveFalse();
        if(!gamePlayTabPanel.activeSelf){
            gamePlayTabPanel.SetActive(true);
        }
        SetHighlightedColor(gamePlayTabButton, 0f);
        gamePlayTabText.color = Color.black;
    }

    private void OnClickControlTabButton(){
        AllPanelSetActiveFalse();
        if(!controlTabPanel.activeSelf){
            controlTabPanel.SetActive(true);
        }
        SetHighlightedColor(controlTabButton, 0f);
        controlTabText.color = Color.black;
    }

    private void AllPanelSetActiveFalse(){
        if(graphicTabPanel.activeSelf){
            graphicTabPanel.SetActive(false);
            SetHighlightedColor(graphicTabButton, 0.1f);
            graphicTabText.color = Color.white;
        }

        if(soundTabPanel.activeSelf){
            soundTabPanel.SetActive(false);
            SetHighlightedColor(soundTabButton, 0.1f);
            soundTabText.color = Color.white;
        }

        if(gamePlayTabPanel.activeSelf){
            gamePlayTabPanel.SetActive(false);
            SetHighlightedColor(gamePlayTabButton, 0.1f);
            gamePlayTabText.color = Color.white;
        }

        if(controlTabPanel.activeSelf){
            controlTabPanel.SetActive(false);
            SetHighlightedColor(controlTabButton, 0.1f);
            controlTabText.color = Color.white;
        }
    }

    private void SetHighlightedColor(Button button, float alpha){
        ColorBlock colors = button.colors;
        Color highlightedColor = colors.highlightedColor;

        highlightedColor.a = alpha;

        colors.highlightedColor = highlightedColor;

        button.colors = colors;
    }

    private void OnClickConfirmButton()
    {
        GetGameSettingsFromUI();
        SettingsManager.Instance.ApplySettings();
        SettingsManager.Instance.SaveSettings();
        // UIManager.Instance.Pop();
    }

    private void OnClickCancelButton()
    {
        if (SettingsManager.Instance.currentGameSettings != null) {
            SettingsManager.Instance.LoadSettings();
            SettingsManager.Instance.ApplySettings();
        }
        UIManager.Instance.Pop();
    }

    // private void OnClickApplyButton()
    // {
    //     SettingsManager.Instance.currentGameSettings = GetGameSettingsFromUI();
    //     SettingsManager.Instance.ApplySettings();
    // }

    private void OnClickResetButton()
    {
        LoadGameSettingsValue(new GameSettings());
    }

    void Setup()
    {
        if (canvas == null)
            throw new NullReferenceException("SettingUIController : canvas가 할당되어있지 않습니다.");
        
        if (resolutionDropdown == null)
            throw new NullReferenceException("SettingUIController : resolutionDropdown이 할당되어있지 않습니다.");

        if (displayModeDropdown == null)
            throw new NullReferenceException("SettingUIController : displayModeDropdown이 할당되어있지 않습니다.");

        if (refreshRateDropdown == null)
            throw new NullReferenceException("SettingUIController : refreshRateDropdown이 할당되어있지 않습니다.");

        if (brightnessSlider == null)
            throw new NullReferenceException("SettingUIController : brightnessSlider가 할당되어있지 않습니다.");
        
        if (masterVolumeSlider == null)
            throw new NullReferenceException("SettingUIController : masterVolumeSlider가 할당되어있지 않습니다.");

        if (bgmVolumeSlider == null)
            throw new NullReferenceException("SettingUIController : bgmVolumeSlider가 할당되어있지 않습니다.");

        if (sfxVolumeSlider == null)
            throw new NullReferenceException("SettingUIController : sfxVolumeSlider가 할당되어있지 않습니다.");

        if (voiceChatOutputDeviceDropdown == null)
            throw new NullReferenceException("SettingUIController : voiceChatOutputDeviceDropdown가 할당되어있지 않습니다.");

        if (voiceChatVolumeSlider == null)
            throw new NullReferenceException("SettingUIController : voiceChatVolumeSlider가 할당되어있지 않습니다.");

        if (microphoneDeviceDropdown == null)
            throw new NullReferenceException("SettingUIController : microphoneDeviceDropdown이 할당되어있지 않습니다.");

        if (microphoneVolumeSlider == null)
            throw new NullReferenceException("SettingUIController : microphoneVolumeSlider가 할당되어있지 않습니다.");

        if (mouseSensitivitySlider == null)
            throw new NullReferenceException("SettingUIController : mouseSensitivitySlider가 할당되어있지 않습니다.");

        if (graphicTabButton == null)
            throw new NullReferenceException("SettingUIController : graphicTabButton 할당되어있지 않습니다.");

        if (soundTabButton == null)
            throw new NullReferenceException("SettingUIController : soundTabButton 할당되어있지 않습니다.");
        
        if (gamePlayTabButton == null)
            throw new NullReferenceException("SettingUIController : gamePlayTabButton 할당되어있지 않습니다.");

        if (controlTabButton == null)
            throw new NullReferenceException("SettingUIController : controlTabButton 할당되어있지 않습니다.");

        if (confirmButton == null)
            throw new NullReferenceException("SettingUIController : confirmButton가 할당되어있지 않습니다.");

        if (cancelButton == null)
            throw new NullReferenceException("SettingUIController : cancelButton가 할당되어있지 않습니다.");

        if (resetButton == null)
            throw new NullReferenceException("SettingUIController : resetButton가 할당되어있지 않습니다.");
        
        
        // 그래픽
        // 해상도 드랍다운 셋업
        var resolutions = Screen.resolutions;
        List<string> resolutionOptions = new List<string>();
        foreach (var resol in resolutions)
        {
            string option = resol.width + " x " + resol.height;
            if (!resolutionOptions.Contains(option))
                resolutionOptions.Add(option);
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);
        
        // 디스플레이 모드 드랍다운 셋업
        displayModeDropdown.ClearOptions();
        displayModeDropdown.AddOptions(
            new List<string>
            {
                "Exclusive Full Screen(Windows Only)",
                "Full Screen Window",
                "Maximized Window(macOS Only)",
                "Windowed"
            }
        );

        // 주사율 드랍다운 셋업
        List<int> refreshRates = new List<int>();
        foreach (var resolution in Screen.resolutions)
        {
            int refreshRate = (int)Math.Ceiling(resolution.refreshRateRatio.value);
            if (!refreshRates.Contains(refreshRate))
                refreshRates.Add(refreshRate);
        }
        refreshRates.Sort();
        List<string> options = refreshRates.ConvertAll(rate => rate + " Hz");
        refreshRateDropdown.ClearOptions();
        refreshRateDropdown.AddOptions(options);

        // 밝기 슬라이더
        brightnessSlider.onValueChanged.AddListener(OnValueChangedBrightness);
        

        // 오디오 설정
        // 볼륨 슬라이더
        masterVolumeSlider.onValueChanged.AddListener(OnValueChangedMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(OnValueChangedBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(OnValueChangedSFXVolume);
        
        // 출력 장치
        RefreshOutputDeviceList(); 
        
        // 보이스챗 볼륨 슬라이더
        voiceChatVolumeSlider.minValue = -50;
        voiceChatVolumeSlider.maxValue =  50;
        voiceChatVolumeSlider.onValueChanged.AddListener(OnValueChangedVoiceChatVolume);
        
        // 입력 장치
        RefreshInputDeviceList();

        // 마이크 볼륨 슬라이더
        microphoneVolumeSlider.minValue = -50;
        microphoneVolumeSlider.maxValue =  50;
        microphoneVolumeSlider.onValueChanged.AddListener(OnValueChangedMicrophoneVolume);
        

        // 게임 플레이
        // 마우스 감도 슬라이더
        mouseSensitivitySlider.minValue = 0.01f;
        mouseSensitivitySlider.maxValue = 20;
        
        
        // 버튼 리스너 추가
        graphicTabButton.onClick.AddListener(OnClickGraphicTabButton);
        soundTabButton.onClick.AddListener(OnClickSoundTabButton);
        gamePlayTabButton.onClick.AddListener(OnClickGamePlayTabButton);
        controlTabButton.onClick.AddListener(OnClickControlTabButton);
        confirmButton.onClick.AddListener(OnClickConfirmButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
        resetButton.onClick.AddListener(OnClickResetButton);
        
        SetHighlightedColor(graphicTabButton, 0f);
        graphicTabText.color = Color.black;
    }

    private void RefreshOutputDeviceList()
    {
        voiceChatOutputDeviceDropdown.Hide();
        voiceChatOutputDeviceDropdown.ClearOptions();
        
        voiceChatOutputDeviceDropdown.options.AddRange(
            VivoxService.Instance.AvailableOutputDevices.Select(
                v => new TMP_Dropdown.OptionData() { text = v.DeviceName }
            )
        );
        
        voiceChatOutputDeviceDropdown.RefreshShownValue();
    }
    
    private void RefreshInputDeviceList()
    {
        microphoneDeviceDropdown.Hide();
        microphoneDeviceDropdown.ClearOptions();

        microphoneDeviceDropdown.options.AddRange(
            VivoxService.Instance.AvailableInputDevices.Select(
                v => new TMP_Dropdown.OptionData() { text = v.DeviceName }
            )
        );
        // microphoneDeviceDropdown.SetValueWithoutNotify(
        //     microphoneDeviceDropdown.options.FindIndex(
        //         option => option.text == VivoxService.Instance.ActiveInputDevice.DeviceName
        //     )
        // );
        microphoneDeviceDropdown.RefreshShownValue();
    }

    void LoadGameSettingsValue(GameSettings gameSettings)
    {
        if (gameSettings == null) return;

        string currentRes = gameSettings.resolutionWidth + " x " + gameSettings.resolutionHeight;
        string currentRefreshRate = gameSettings.refreshRate.ToString() + " Hz";
        
        // 비디오
        SetDropdownValueToTarget(resolutionDropdown, currentRes);
        displayModeDropdown.value = gameSettings.displayMode;
        SetDropdownValueToTarget(refreshRateDropdown, currentRefreshRate);
        brightnessSlider.value = gameSettings.brightness;
        
        // 오디오
        masterVolumeSlider.value = gameSettings.masterVolume;
        bgmVolumeSlider.value = gameSettings.bgmVolume;
        sfxVolumeSlider.value = gameSettings.sfxVolume;
        SetDropdownValueToTarget(voiceChatOutputDeviceDropdown, gameSettings.voiceChatOutputDevice);
        voiceChatVolumeSlider.value = gameSettings.voiceChatVolume;
        SetDropdownValueToTarget(microphoneDeviceDropdown, gameSettings.microphoneDevice);
        microphoneVolumeSlider.value = gameSettings.microphoneVolume;
        
        // 컨트롤
        mouseSensitivitySlider.value = gameSettings.mouseSensitivity;
    }

    /// <summary>
    /// TMP_Dropdown 원하는 데이터로 값을 변경하는 메소드
    /// </summary>
    /// <param name="dropdown">값을 변경하고 싶은 TMP_Dropdown</param>
    /// <param name="targetData">타겟 데이터</param>
    void SetDropdownValueToTarget(TMP_Dropdown dropdown, string target)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            var dropdownData = dropdown.options[i].text;
            if (Equals(target, dropdownData))
            {
                dropdown.value = i;
                return;
            }
        }

        // 찾는 데이터가 없는 경우
        Debug.LogWarning($"SetDropdownValueToTarget : {dropdown} 에 {target} 값이 없습니다.");
        dropdown.value = 0;
    }

    GameSettings GetGameSettingsFromUI()
    {
        var settings = SettingsManager.Instance.currentGameSettings;

        string[] resolutionParts = resolutionDropdown.captionText.text.Split('x');
        string refreshRateString = refreshRateDropdown.captionText.text.Replace("Hz", "").Trim();
        settings.resolutionWidth  = int.Parse(resolutionParts[0].Trim());
        settings.resolutionHeight = int.Parse(resolutionParts[1].Trim());
        settings.displayMode = displayModeDropdown.value;
        settings.refreshRate = int.Parse(refreshRateString);
        settings.brightness = Mathf.Round(brightnessSlider.value * 100f) / 100f;

        settings.masterVolume = masterVolumeSlider.value;
        settings.bgmVolume = bgmVolumeSlider.value;
        settings.sfxVolume = sfxVolumeSlider.value;
        settings.voiceChatOutputDevice = voiceChatOutputDeviceDropdown.captionText.text;
        settings.microphoneDevice = microphoneDeviceDropdown.captionText.text;

        settings.mouseSensitivity = Mathf.Round(mouseSensitivitySlider.value * 100f) / 100f;

        return settings;
    }

    // 사운드 탭__________________________________________________________________________________________________________________________________________
    private void OnValueChangedMasterVolume(float value) {
        AudioMixerController.Instance.SetMasterVolume(value);
        // 실시간으로 슬라이드에 따라 변화하는 것을 저장
        SettingsManager.Instance.currentGameSettings.masterVolume = value;
    }

    private void OnValueChangedBGMVolume(float value)
    {
        AudioMixerController.Instance.SetBGMVolume(value);
        SettingsManager.Instance.currentGameSettings.bgmVolume = value;
    }

    private void OnValueChangedSFXVolume(float value)
    {
        AudioMixerController.Instance.SetSFXVolume(value);
        SettingsManager.Instance.currentGameSettings.sfxVolume = value;
    }

    private void OnValueChangedVoiceChatVolume(float value) {
        int volume = Mathf.RoundToInt(value);
        //실시간으로 슬라이드에 따라 변화하는 것을 저장
        SettingsManager.Instance.currentGameSettings.voiceChatVolume = volume;
        VivoxManager.Instance.SetOutputDeviceVolume(volume);
    }

    private void OnValueChangedMicrophoneVolume(float value) {
        int volume = Mathf.RoundToInt(value);
        //실시간으로 슬라이드에 따라 변화하는 것을 저장
        SettingsManager.Instance.currentGameSettings.microphoneVolume = volume;
        VivoxManager.Instance.SetInputDeviceVolume(volume);
    }

    private void OnValueChangedBrightness(float value) {
        float brightness = Mathf.Round(brightnessSlider.value * 100f) / 400f + 0.5f;
        //실시간으로 슬라이드에 따라 변화하는 것을 저장
        SettingsManager.Instance.currentGameSettings.brightness = brightness;
        
        if (GameObject.Find("Global Volume") != null && GameObject.Find("Global Volume").TryGetComponent(out GlobalVolumeController gvc))
            gvc.ChangeGammaOffsetValue(value);
    }
}
