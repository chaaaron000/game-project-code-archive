using UnityEngine;
using UnityEngine.Events;

namespace Nostal.Settings
{
    [CreateAssetMenu(fileName = "SO_SoundSettings", menuName = "Settings/SoundSettings", order = 1)]
    public class SoundSettingsSO : SettingsSO
    {
        private const float  DEFAULT_MASTER_VOLUME            = 0.5f;
        private const float  DEFAULT_BGM_VOLUME               = 0.5f;
        private const float  DEFAULT_SFX_VOLUME               = 0.5f;
        private const float  DEFAULT_VOICE_CHAT_VOLUME        = 0f;
        private const float  DEFAULT_MICROPHONE_VOLUME        = 0f;
        private const string DEFAULT_VOICE_CHAT_OUTPUT_DEVICE = "Default System Device";
        private const string DEFAULT_MICROPHONE_DEVICE        = "Default System Device";

        public UnityAction<float>  OnVoiceChatVolumeChanged;
        public UnityAction<float>  OnMicrophoneVolumeChanged;
        public UnityAction<string> OnVoiceChatOutputDeviceChanged;
        public UnityAction<string> OnMicrophoneDeviceChanged;

        [Header("Settings")] 
        [SerializeField] 
        private float m_masterVolume = 0.5f;
        public  float   MasterVolume
        {
            get => m_masterVolume;
            set
            {
                m_masterVolume = value;
                AudioMixerController.Instance.SetMasterVolume(m_masterVolume);
                Save();
            }
        }

        [SerializeField] 
        private float m_bgmVolume = 0.5f;
        public  float   BgmVolume 
        {
            get => m_bgmVolume;
            set
            {
                m_bgmVolume = value;
                AudioMixerController.Instance.SetBGMVolume(m_bgmVolume);
                Save();
            }
        }

        [SerializeField] 
        private float m_sfxVolume = 0.5f;
        public  float   SfxVolume
        {
            get => m_sfxVolume;
            set
            {
                m_sfxVolume = value;
                AudioMixerController.Instance.SetSFXVolume(m_sfxVolume);
                Save();
            }
        }
        
        [SerializeField]
        private float m_voiceChatVolume = 0f;
        public  float   VoiceChatVolume
        {
            get => m_voiceChatVolume;
            set
            {
                m_voiceChatVolume = value;
                OnVoiceChatVolumeChanged?.Invoke(m_voiceChatVolume);
                Save();
            }
        }
        
        [SerializeField]
        private float m_microphoneVolume = 0f;
        public  float   MicrophoneVolume
        {
            get => m_microphoneVolume;
            set
            {
                m_microphoneVolume = value;
                OnMicrophoneVolumeChanged?.Invoke(m_microphoneVolume);
                Save();
            }
        }
        
        [SerializeField]
        private string m_voiceChatOutputDevice = "Default System Device";
        public  string   VoiceChatOutputDevice
        {
            get => m_voiceChatOutputDevice;
            set
            {
                // 변경하려는 값이 같은 경우 
                if (value == m_voiceChatOutputDevice)
                {
                    return;
                }
                
                m_voiceChatOutputDevice = value;
                OnVoiceChatOutputDeviceChanged?.Invoke(m_voiceChatOutputDevice);
                Save();
            }
        }
        
        [SerializeField]
        private string m_microphoneDevice = "Default System Device";
        public  string   MicrophoneDevice
        {
            get => m_microphoneDevice;
            set
            {
                // 변경하려는 값이 같은 경우 
                if (value == m_microphoneDevice)
                {
                    return;
                }
                
                m_microphoneDevice = value;
                OnMicrophoneDeviceChanged?.Invoke(m_microphoneDevice);
                Save();
            }
        }

        public override void Load()
        {
            base.Load();
            
            AudioMixerController.Instance.SetMasterVolume(m_masterVolume);
            AudioMixerController.Instance.SetBGMVolume(m_bgmVolume);
            AudioMixerController.Instance.SetSFXVolume(m_sfxVolume);
        }
    }
}