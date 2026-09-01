using System;
using System.Linq;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

namespace Nostal.Settings
{
    public class SoundSettingsUIView : MonoBehaviour
    {
        [SerializeField] public Slider MasterVolumeSlider;
        [SerializeField] public Slider BgmVolumeSlider;
        [SerializeField] public Slider SfxVolumeSlider;
        [SerializeField] public Slider VoiceChatVolumeSlider;
        [SerializeField] public Slider MicrophoneVolumeSlider;
        [SerializeField] public TMP_Dropdown VoiceChatOutputDeviceDropdown;
        [SerializeField] public TMP_Dropdown MicrophoneDeviceDropdown;

        public void Setup()
        {
            RefreshOutputDeviceList();
            RefreshInputDeviceList();
            
            VoiceChatVolumeSlider.minValue  = -50;
            VoiceChatVolumeSlider.maxValue  =  50;
            
            MicrophoneVolumeSlider.minValue = -50;
            MicrophoneVolumeSlider.maxValue =  50;
        }

        private void RefreshOutputDeviceList()
        {
            VoiceChatOutputDeviceDropdown.Hide();
            VoiceChatOutputDeviceDropdown.ClearOptions();
            VoiceChatOutputDeviceDropdown.options.AddRange(
                VivoxService.Instance.AvailableOutputDevices.Select(v =>
                    new TMP_Dropdown.OptionData() { text = v.DeviceName }));

            VoiceChatOutputDeviceDropdown.RefreshShownValue();
        }

        private void RefreshInputDeviceList()
        {
            MicrophoneDeviceDropdown.Hide();
            MicrophoneDeviceDropdown.ClearOptions();
            MicrophoneDeviceDropdown.options.AddRange(
                VivoxService.Instance.AvailableInputDevices.Select(v =>
                    new TMP_Dropdown.OptionData() { text = v.DeviceName }));

            MicrophoneDeviceDropdown.RefreshShownValue();
        }
    }
}