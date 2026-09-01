using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("오디오 믹서 연결")]
    [SerializeField]
    private AudioMixer audioMixer;

    [Header("UI 연결")]
    [SerializeField]
    private Slider bgmSlider;

    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private Slider masterSlider;

    [SerializeField]
    private TMP_Dropdown resolutionDropdown;

    [Header("UI 텍스트(숫자) 연결")]
    [SerializeField]
    private TMP_Text masterVolText;

    [SerializeField]
    private TMP_Text bgmVolText;

    [SerializeField]
    private TMP_Text sfxVolText;

    private List<Resolution> resolutions;
    private int currentResolutionIndex = 0;

    private void Start()
    {
        InitResolutionDropdown();
        LoadSettings();
    }

    // --- 소리 실시간 조절 함수 ---
    // 슬라이더의 OnValueChanged 이벤트에 연결할 함수들입니다.
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
        //슬라이더 값(0~1)을 0~100으로 곱하고 반올림해서 텍스트로 표시!
        masterVolText.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVol", Mathf.Log10(volume) * 20);
        bgmVolText.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        sfxVolText.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    // --- 해상도 세팅 ---
    private void InitResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string> { "1920 x 1080", "1600 x 900", "1280 x 720" };
        resolutionDropdown.AddOptions(options);
        // JSON에서 해상도 불러오기
        resolutionDropdown.value = DataManager.Instance.SaveData.resolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // --- 버튼 기능들 ---
    public void OnClickApply()
    {
        // 1. 해상도 적용
        int resIndex = resolutionDropdown.value;
        if (resIndex == 0)
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        else if (resIndex == 1)
            Screen.SetResolution(1600, 900, FullScreenMode.FullScreenWindow);
        else if (resIndex == 2)
            Screen.SetResolution(1280, 720, FullScreenMode.FullScreenWindow);

        // 2. DataManager에 값 넣기 (PlayerPrefs 대체)
        DataManager.Instance.SaveData.resolutionIndex = resIndex;
        DataManager.Instance.SaveData.masterVolume = masterSlider.value;
        DataManager.Instance.SaveData.bgmVolume = bgmSlider.value;
        DataManager.Instance.SaveData.sfxVolume = sfxSlider.value;

        // 3. JSON으로 실제 파일 저장!
        DataManager.Instance.SaveGameData();

        DebugConsole.Log("환경 설정 JSON 적용 완료");
    }

    public void OnClickResetToDefault()
    {
        // 기획 기본값으로 세팅 (80%, 90%, 70%)
        masterSlider.value = 0.8f;
        bgmSlider.value = 0.9f;
        sfxSlider.value = 0.7f;

        resolutionDropdown.value = 0; // 1920x1080
        resolutionDropdown.RefreshShownValue();

        // 믹서에도 즉시 반영
        SetMasterVolume(0.8f);
        SetBGMVolume(0.9f);
        SetSFXVolume(0.7f);
    }

    public void OnClickBack()
    {
        // 기존에 만드신 GameSceneManager를 활용하여 타이틀 씬으로 돌아갑니다.
        GameSceneManager.Instance.ChangeScene(SceneType.TITLE);
    }

    private void LoadSettings()
    {
        // JSON에서 볼륨 불러오기
        masterSlider.value = DataManager.Instance.SaveData.masterVolume;
        bgmSlider.value = DataManager.Instance.SaveData.bgmVolume;
        sfxSlider.value = DataManager.Instance.SaveData.sfxVolume;
        // 슬라이더 값을 불러온 후 믹서에도 적용해줍니다.
        SetMasterVolume(masterSlider.value);
        SetBGMVolume(bgmSlider.value);
        SetSFXVolume(sfxSlider.value);
    }
}
