using System;
using System.Collections;
using System.Collections.Generic;
using Nostal.Settings;
using UnityEngine;
using UnityEngine.Serialization;

public class GraphicsSettingsUIPresenter : MonoBehaviour
{
    [SerializeField] private GraphicsSettingsSO m_graphicsSettingsSO;
    [SerializeField] private GraphicsSettingsUIView m_view;

    private void OnEnable()
    {
        m_view.Setup();
        string currentRes = m_graphicsSettingsSO.ResolutionWidth + " x " + m_graphicsSettingsSO.ResolutionHeight;
        string currentRefreshRate = m_graphicsSettingsSO.ResolutionRefreshRate.ToString() + " Hz";
        UIUtility.SetDropdownValueToTarget(m_view.ResolutionDropdown, currentRes);
        UIUtility.SetDropdownValueToTarget(m_view.RefreshRateDropdown, currentRefreshRate);
        m_view.DisplayModeDropdown.value = m_graphicsSettingsSO.DisplayMode;
        m_view.BrightnessSlider.value = m_graphicsSettingsSO.Brightness;
        
        // m_view.ResolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        // m_view.DisplayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
        // m_view.RefreshRateDropdown.onValueChanged.AddListener(OnRefreshRateChanged);
        // m_view.BrightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    // private void OnDestroy()
    // {
    //     m_view.ResolutionDropdown.onValueChanged.RemoveAllListeners();
    //     m_view.DisplayModeDropdown.onValueChanged.RemoveAllListeners();
    //     m_view.RefreshRateDropdown.onValueChanged.RemoveAllListeners();
    //     m_view.BrightnessSlider.onValueChanged.RemoveAllListeners();
    // }

    private void OnResolutionChanged(int index)
    {
        string[] resolutionParts = m_view.ResolutionDropdown.options[index].text.Split('x');
        m_graphicsSettingsSO.ResolutionWidth  = int.Parse(resolutionParts[0].Trim());
        m_graphicsSettingsSO.ResolutionHeight = int.Parse(resolutionParts[1].Trim());
    }

    private void OnDisplayModeChanged(int index)
    {
        m_graphicsSettingsSO.DisplayMode = index;
    }

    private void OnRefreshRateChanged(int index)
    {
        string refreshRateString = m_view.RefreshRateDropdown.options[index].text.Replace("Hz", "").Trim();
        m_graphicsSettingsSO.ResolutionRefreshRate = int.Parse(refreshRateString);
    }

    private void OnBrightnessChanged(float value)
    {
        m_graphicsSettingsSO.Brightness = value;
    }
}
