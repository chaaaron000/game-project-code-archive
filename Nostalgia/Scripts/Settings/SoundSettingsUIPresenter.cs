using System;
using UnityEngine;

namespace Nostal.Settings
{
    public class SoundSettingsUIPresenter : MonoBehaviour
    {
        [SerializeField] private SoundSettingsSO m_soundSettingsSO;
        [SerializeField] private SoundSettingsUIView m_view;

        private void OnEnable()
        {
            m_view.Setup();
            m_view.MasterVolumeSlider.value     = m_soundSettingsSO.MasterVolume;
            m_view.BgmVolumeSlider.value        = m_soundSettingsSO.BgmVolume;
            m_view.SfxVolumeSlider.value        = m_soundSettingsSO.SfxVolume;
            m_view.VoiceChatVolumeSlider.value  = m_soundSettingsSO.VoiceChatVolume;
            m_view.MicrophoneVolumeSlider.value = m_soundSettingsSO.MicrophoneVolume;
            UIUtility.SetDropdownValueToTarget(m_view.VoiceChatOutputDeviceDropdown, m_soundSettingsSO.VoiceChatOutputDevice);
            UIUtility.SetDropdownValueToTarget(m_view.MicrophoneDeviceDropdown,      m_soundSettingsSO.MicrophoneDevice);
            
            // m_view.MasterVolumeSlider.onValueChanged.AddListener(OnValueChangedMasterVolume);
            // m_view.BgmVolumeSlider.onValueChanged.AddListener(OnValueChangedBGMVolume);
            // m_view.SfxVolumeSlider.onValueChanged.AddListener(OnValueChangedSFXVolume);
            // m_view.VoiceChatVolumeSlider.onValueChanged.AddListener(OnValueChangedVoiceChatVolume);
            // m_view.MicrophoneVolumeSlider.onValueChanged.AddListener(OnValueChangedMicrophoneVolume);
        }

        // private void OnDestroy()
        // {
        //     m_view.MasterVolumeSlider.onValueChanged.RemoveAllListeners();
        //     m_view.BgmVolumeSlider.onValueChanged.RemoveAllListeners();
        //     m_view.SfxVolumeSlider.onValueChanged.RemoveAllListeners();
        //     m_view.VoiceChatVolumeSlider.onValueChanged.RemoveAllListeners();
        //     m_view.MicrophoneVolumeSlider.onValueChanged.RemoveAllListeners();
        // }

        public void OnValueChangedMasterVolume(float value)
        {
            m_soundSettingsSO.MasterVolume = value;
        }

        public void OnValueChangedBGMVolume(float value)
        {
            m_soundSettingsSO.BgmVolume = value;
        }
        
        public void OnValueChangedSFXVolume(float value)
        {
            m_soundSettingsSO.SfxVolume = value;
        }
        
        public void OnValueChangedVoiceChatVolume(float value)
        {
            m_soundSettingsSO.VoiceChatVolume = value;
        }
        
        public void OnValueChangedMicrophoneVolume(float value)
        {
            m_soundSettingsSO.MicrophoneVolume = value;
        }

        public void OnValueChangedVoiceChatOutputDevice(int index)
        {
            m_soundSettingsSO.VoiceChatOutputDevice = m_view.VoiceChatOutputDeviceDropdown.options[index].text;
        }

        public void OnValueChangedMicrophoneDevice(int index)
        {
            m_soundSettingsSO.MicrophoneDevice = m_view.MicrophoneDeviceDropdown.options[index].text;
        }
    }
}