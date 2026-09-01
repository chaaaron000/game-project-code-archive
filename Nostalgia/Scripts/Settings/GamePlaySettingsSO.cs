using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Nostal.Settings
{
    [CreateAssetMenu(fileName = "SO_GamePlaySettings", menuName = "Settings/GamePlaySettings", order = 2)]
    public class GamePlaySettingsSO : SettingsSO
    {
        private const float DEFAULT_MOUSE_SENSITIVITY = 5f;

        [SerializeField] 
        private float m_mouseSensitivity = 5f;
        public  float   MouseSensitivity
        {
            get => m_mouseSensitivity;
            set
            {
                m_mouseSensitivity = value;
                Save();
            }
        }

        [SerializeField] private int m_languageLocaleIndex = 0;
        public int LanguageLocaleIndex
        {
            get => m_languageLocaleIndex;
            set
            {
                m_languageLocaleIndex = value;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[m_languageLocaleIndex];
                Save();
            }
        }

        public override void Load()
        {
            base.Load();
            
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[m_languageLocaleIndex];
        }
    }
}