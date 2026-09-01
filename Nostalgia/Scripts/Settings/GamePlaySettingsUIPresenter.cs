using System;
using System.Collections;
using System.Collections.Generic;
using Nostal.Settings;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GamePlaySettingsUIPresenter : MonoBehaviour
{
    [SerializeField] public GamePlaySettingsSO m_gamePlaySettingsSO;
    [SerializeField] private GamePlaySettingsUIView m_view;

    private void OnEnable()
    {
        m_view.MouseSensitivitySlider.value = m_gamePlaySettingsSO.MouseSensitivity;
        m_view.LanguageDropdown.value = m_gamePlaySettingsSO.LanguageLocaleIndex;
    }

    public void OnMouseSensitivityChanged(float value)
    {
        m_gamePlaySettingsSO.MouseSensitivity = value;
    }

    public void OnLanguageChanged(int index)
    {
        if (index < 0 && index >= LocalizationSettings.AvailableLocales.Locales.Count)
        {
            return;
        }
        
        m_gamePlaySettingsSO.LanguageLocaleIndex = index;
    }
}
