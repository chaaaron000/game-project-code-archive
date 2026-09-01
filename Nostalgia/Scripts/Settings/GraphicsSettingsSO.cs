using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Nostal.Settings
{
    [CreateAssetMenu(fileName = "SO_GraphicsSettings", menuName = "Settings/GraphicsSettings", order = 0)]
    public class GraphicsSettingsSO : SettingsSO
    {
        public UnityAction<float> OnBrightnessChanged;

        private const int     DEFAULT_RESOLUTION_WIDTH          = 1920;
        private const int     DEFAULT_RESOLUTION_HEIGHT         = 1080;
        private const int     DEFAULT_RESOLUTION_REFRESH_RATE   = 60;
        private const int     DEFAULT_DISPLAY_MODE              = 3;
        private const float   DEFAULT_BRIGHTNESS                = 0.5f;
        
        [SerializeField]
        private int m_resolutionWidth;
        public  int   ResolutionWidth
        {
            get => m_resolutionWidth;
            set
            {
                m_resolutionWidth = value;
                ChangeScreenSettings();
                Save();
            }
        }
        
        [SerializeField]
        private int m_resolutionHeight;
        public  int   ResolutionHeight
        {
            get => m_resolutionHeight;
            set
            {
                m_resolutionHeight = value;
                ChangeScreenSettings();
                Save();
            }
        }
        
        [SerializeField]
        private int m_resolutionRefreshRate;
        public  int   ResolutionRefreshRate
        {
            get => m_resolutionRefreshRate;
            set
            {
                m_resolutionRefreshRate = value;
                ChangeScreenSettings();
                Save();
            }
        }

        [SerializeField] 
        private int m_displayMode;  // 화면 모드
        public  int   DisplayMode
        {
            get => m_displayMode;
            set
            {
                m_displayMode = value;
                ChangeScreenSettings();
                Save();
            }
        }

        [SerializeField] 
        private float m_brightness;
        public  float   Brightness
        {
            get => m_brightness;
            set
            {
                m_brightness = value;
                OnBrightnessChanged?.Invoke(m_brightness);
                Save();
            }
        }

        public override void Load()
        {
            base.Load();
            
            ChangeScreenSettings();
        }

        private void ChangeScreenSettings()
        {
            FullScreenMode mode = (FullScreenMode)Mathf.Clamp(m_displayMode, 0, 3);
            RefreshRate rr =
                new RefreshRate() { numerator = Convert.ToUInt32(m_resolutionRefreshRate), denominator = 1 };
            Screen.SetResolution(m_resolutionWidth, m_resolutionHeight, mode, rr);
        }
    }
}