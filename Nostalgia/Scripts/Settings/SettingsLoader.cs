using Nostal.Settings;
using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    [SerializeField] private SettingsSO[] m_settingsSO;

    private void Start()
    {
        foreach (SettingsSO so in m_settingsSO)
        {
            so.Load();
        }
    }
}
