using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nostal.Network;
using Nostal.Settings;
using Nostal.Util;
using UnityEngine;
using Unity.Services.Vivox;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Localization.Settings;

[Serializable]
public class PositionalChannelSettings
{
    [Tooltip(@"청자가 화자의 음성을 들을 수 있고 텍스트 메시지를 수신할 수 있는 화자로부터의 최대 거리입니다.
기본값은 32입니다.")]
    [SerializeField] private int AudibleDistance = 32;
    
    [Tooltip(@"영역 내에서는 화자의 오디오가 원래 음량을 유지하고, 영역을 벗어나면 음성 채팅의 소리 크기가 페이드아웃되기 시작하는 것처럼 들리는 영역을 조정합니다.
기본값은 1입니다. 이 값은 0 <= ConversationalDistance <= AudibleDistance 범위의 정수여야 합니다.")]
    [SerializeField] private int ConversationalDistance = 1;
    
    [Tooltip(@"AudioFadeModel 커브의 진폭을 조정하여 음성 채팅 소리 크기의 감쇠 정도를 조정합니다.
기본값은 1.0입니다.")]
    [SerializeField] private float AudioFadeIntensityByDistance = 1.0f;

    public Channel3DProperties GetChannel3DProperties()
    {
        return new Channel3DProperties(
            AudibleDistance, 
            ConversationalDistance, 
            AudioFadeIntensityByDistance,
            AudioFadeModel.LinearByDistance
        );
    }
}

public class VivoxManager : Singleton<VivoxManager>
{
    public bool VivoxReady { get; private set; }
    
    public GameObject SpeakingPositionGameObject;

    [Header("Sound Settings Scriptable Object")]
    [SerializeField] private SoundSettingsSO m_soundSettingsSO;
    
    [Header("Voice Chat Settings")]
    [SerializeField] private PositionalChannelSettings positionalChannelSettings;

    [Tooltip("보이스 포지션이 업데이트 되는 빈도")]
    [SerializeField] private float positionUpdateRate = 0.015f;

    [SerializeField] private string m_vivoxLoginName;

    // 로그인 시도
    private const int MAX_LOGIN_ATTEMPTS = 10;
    private string m_loginStatusMessage;

    // 채널 정보
    private bool m_bIsChannelJoined;
    private string m_vivoxChannelName;
    
    private TaskCompletionSource<bool> vivoxInitializedTask = new TaskCompletionSource<bool>();

    private void OnEnable()
    {
        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        m_soundSettingsSO.OnVoiceChatVolumeChanged       += SetOutputDeviceVolume;
        m_soundSettingsSO.OnMicrophoneVolumeChanged      += SetInputDeviceVolume;
        m_soundSettingsSO.OnVoiceChatOutputDeviceChanged += ChangeOutputDevice;
        m_soundSettingsSO.OnMicrophoneDeviceChanged      += ChangeInputDevice;
        
        m_bIsChannelJoined = false;
    }

    private async void Start()
    {
        SpeakingPositionGameObject = gameObject;
        
        // 유니티 서비스 초기화
        await UnityServices.InitializeAsync();
        
        // AuthenticationService를 사용하여 익명 인증
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // Vivox 초기화
        await VivoxService.Instance.InitializeAsync();
        vivoxInitializedTask.SetResult(true);

        // Vivox 콜백 등록
        VivoxService.Instance.ChannelJoined += OnChannelJoined;
        VivoxService.Instance.ChannelLeft += ChannelLeft;

        // 입출력 장치 설정 및 볼륨 설정
        ChangeOutputDevice(m_soundSettingsSO.VoiceChatOutputDevice);
        ChangeInputDevice(m_soundSettingsSO.MicrophoneDevice);
        SetOutputDeviceVolume(m_soundSettingsSO.VoiceChatVolume);
        SetInputDeviceVolume(m_soundSettingsSO.MicrophoneVolume);
        
        Debug.Log("VivoxManager: Vivox 초기화 완료");
        
        // 테스트 로그인
        string steamId = SteamUser.GetSteamID().GetAccountID().ToString();
        string steamName = SteamFriends.GetPersonaName();
        m_vivoxLoginName = steamId + steamName;
        await LoginToVivoxAsync(m_vivoxLoginName);
    }

    private void Update()
    {
        if (!VivoxReady)
        {
            return;
        }
        
        if (m_bIsChannelJoined)
        {
            VivoxService.Instance.Set3DPosition(SpeakingPositionGameObject, m_vivoxChannelName);
        }
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // Vivox 콜백 해제
        VivoxService.Instance.ChannelJoined -= OnChannelJoined;
        VivoxService.Instance.ChannelLeft -= ChannelLeft;
        
        m_soundSettingsSO.OnVoiceChatVolumeChanged       -= SetOutputDeviceVolume;
        m_soundSettingsSO.OnMicrophoneVolumeChanged      -= SetInputDeviceVolume;
        m_soundSettingsSO.OnVoiceChatOutputDeviceChanged -= ChangeOutputDevice;
        m_soundSettingsSO.OnMicrophoneDeviceChanged      -= ChangeInputDevice;
    }

    /// <summary>
    /// Vivox 사용을 위한 로그인 async 메소드
    /// </summary>
    /// <param name="userName">Steam 사용자 닉네임 넣을 것</param>
    private async Task LoginToVivoxAsync(string userName)
    {
        // 로그인 옵션 생성
        LoginOptions options = new LoginOptions
        {
            DisplayName = userName,
            EnableTTS = true
        };

        // 로그인 시도
        for (int i = 0; i < MAX_LOGIN_ATTEMPTS; i++)
        {
            string attemptMessage = $"Attempting to log in to Vivox... ({i + 1} / {MAX_LOGIN_ATTEMPTS})";
            m_loginStatusMessage = attemptMessage;
            Debug.Log(attemptMessage, this);

            // 로그인시 발생하는 예외 처리
            try
            {
                await VivoxService.Instance.LoginAsync(options);
                
                string successMessage = "Vivox login successful";
                m_loginStatusMessage = successMessage;
                Debug.Log(successMessage, this);
                VivoxReady = true;
                
                StartCoroutine(ClearLoginStatusMessageAfterDelay(5f));
                return;
            }
            catch (Exception e)
            {
                // VivoxApiException에 직접 접근할 수 없으므로 예외 메시지에서 스테이터스 코드를 확인합니다.
                if (e.Message.Contains("10028")) // 10028: HTTP Timeout
                {
                    string timeoutMessage = $"Vivox login failed (HTTP Timeout). ({i + 1}/{MAX_LOGIN_ATTEMPTS})";
                    m_loginStatusMessage = timeoutMessage;
                    Debug.LogWarning(timeoutMessage);
                    await Task.Delay(5000);
                }
                else
                {
                    string errorMessage = $"Vivox login failed: {e.Message}. ({i + 1}/{MAX_LOGIN_ATTEMPTS})";
                    m_loginStatusMessage = errorMessage;
                    Debug.LogWarning(errorMessage, this);
                    await Task.Delay(5000);
                }
            }
        }

        string finalErrorMessage = "Vivox login failed";
        m_loginStatusMessage = finalErrorMessage;
        Debug.LogError(finalErrorMessage, this);
    }

    /// <summary>
    /// 그룹 보이스 채널에 참가하기 위한 async 메소드
    /// </summary>
    /// <param name="channelName">방 생성 코드 입력할 것</param>
    public async void JoinGroupVoiceChannel(string channelName)
    {
        await VivoxService.Instance.JoinGroupChannelAsync(channelName, ChatCapability.TextAndAudio);
    }

    /// <summary>
    /// 3D 보이스 채널에 참가하기 위한 async 메소드. 이후 위치 업데이트 코루틴 실행됨.
    /// </summary>
    /// <param name="channelName">방 생성 코드 입력할 것</param>
    public async void JoinPositionalChannelAsync(string channelName)
    {
        await vivoxInitializedTask.Task;
        
        await VivoxService.Instance.JoinPositionalChannelAsync(
            channelName, 
            ChatCapability.AudioOnly, 
            positionalChannelSettings.GetChannel3DProperties()
        );
    }

    public async Task LeaveAllChannelAsync()
    {
        // 채널을 완전히 나갈 때까지 게임 버튼 비활성화
        VivoxReady = false;
        // if (m_mainMenuUIController != null)
        //     m_mainMenuUIController.TryGameButtonInteractable();
        
        await VivoxService.Instance.LeaveAllChannelsAsync();

        VivoxReady = true;
        // if (m_mainMenuUIController != null)
        //     m_mainMenuUIController.TryGameButtonInteractable();
    }
    
    public async void ChangeOutputDevice(string deviceName)
    {
        await vivoxInitializedTask.Task;

        VivoxOutputDevice device =
            VivoxService.Instance.AvailableOutputDevices.FirstOrDefault(outputDevice =>
                outputDevice.DeviceName == deviceName);

        // 만약 같은 이름의 디바이스가 없으면 0 번째 디바이스로 바꾸고 세팅 저장
        if (device == null)
        {
            device = VivoxService.Instance.AvailableOutputDevices[0];
            m_soundSettingsSO.VoiceChatOutputDevice = deviceName;
        }
        
        await VivoxService.Instance.SetActiveOutputDeviceAsync(device);
    }

    public async void SetOutputDeviceVolume(float value)
    {
        await vivoxInitializedTask.Task;
        VivoxService.Instance.SetOutputDeviceVolume((int)value);
    }
    
    public async void ChangeInputDevice(string deviceName)
    {
        await vivoxInitializedTask.Task;
        
        VivoxInputDevice device = 
            VivoxService.Instance.AvailableInputDevices.FirstOrDefault(inputDevice => 
                inputDevice.DeviceName == deviceName);
        
        // 만약 같은 이름의 디바이스가 없으면 0 번째 디바이스로 바꾸고 세팅 저장
        if (device == null)
        {
            device = VivoxService.Instance.AvailableInputDevices[0];
            m_soundSettingsSO.MicrophoneDevice = device.DeviceName;
        }
        
        await VivoxService.Instance.SetActiveInputDeviceAsync(device);
    }
    
    public async void SetInputDeviceVolume(float value)
    {
        await vivoxInitializedTask.Task;
        VivoxService.Instance.SetInputDeviceVolume((int)value);
    }

    /// <summary>
    /// positionUpdateRate 마다 3D 보이스의 위치를 SpeakingPositionGameObject로 업데이트 하는 코루틴 메소드
    /// </summary>
    /// <param name="channelName">방 생성 코드 입력할 것</param>
    /// <returns></returns>
    private IEnumerator UpdateVoice3DPosition(string channelName)
    {
        while (true)
        {
            VivoxService.Instance.Set3DPosition(SpeakingPositionGameObject, channelName);
            yield return new WaitForSeconds(positionUpdateRate);
        }
    }

    private void OnChannelJoined(string channelName)
    {
        Debug.Log($"Vivox Channel Joined: {channelName}", this);

        m_vivoxChannelName = channelName;
        m_bIsChannelJoined = true;
        // m_updateVoicePositionCoroutine = StartCoroutine(UpdateVoice3DPosition(channelName));
    }

    private void ChannelLeft(string channelName)
    {
        Debug.Log($"Vivox Channel Left: {channelName}", this);

        m_bIsChannelJoined = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpeakingPositionGameObject = Camera.main?.gameObject ?? gameObject;
        
        // if ((SpeakingPositionGameObject = Camera.main.gameObject) == null)
        // {
        //     SpeakingPositionGameObject = gameObject;
        //     Debug.LogWarning("보이스챗 위치 동기화를 위한 Main Camera를 찾을 수 없습니다.");
        // }
    }

    private void OnGUI()
    {
        if (!string.IsNullOrEmpty(m_loginStatusMessage))
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.LowerRight;
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(Screen.width - 300, Screen.height - 30, 300, 30), m_loginStatusMessage, style);
        }
    }

    /// <summary>
    /// 로그인 상태 메시지를 일정 시간 후에 지우는 코루틴
    /// </summary>
    /// <param name="delay">지연 시간 (초)</param>
    /// <returns></returns>
    private IEnumerator ClearLoginStatusMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_loginStatusMessage = string.Empty;
    }
}
