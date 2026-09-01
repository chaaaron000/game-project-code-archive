using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsUIView : MonoBehaviour
{
    [SerializeField] public TMP_Dropdown ResolutionDropdown;
    [SerializeField] public TMP_Dropdown DisplayModeDropdown;
    [SerializeField] public TMP_Dropdown RefreshRateDropdown;
    [SerializeField] public Slider BrightnessSlider;

    public void Setup()
    {
        // 해상도 드랍다운 셋업
        var resolutions = Screen.resolutions;
        List<string> resolutionOptions = new List<string>();
        foreach (var resol in resolutions)
        {
            string option = resol.width + " x " + resol.height;
            if (!resolutionOptions.Contains(option)) resolutionOptions.Add(option);
        }

        ResolutionDropdown.ClearOptions();
        ResolutionDropdown.AddOptions(resolutionOptions);

        // 디스플레이 모드 드랍다운 셋업
        DisplayModeDropdown.ClearOptions();
        DisplayModeDropdown.AddOptions(new List<string>
        {
            "Exclusive Full Screen(Windows Only)", "Full Screen Window", "Maximized Window(macOS Only)", "Windowed"
        });

        // 주사율 드랍다운 셋업
        List<int> refreshRates = new List<int>();
        foreach (var resolution in Screen.resolutions)
        {
            int refreshRate = (int)Math.Ceiling(resolution.refreshRateRatio.value);
            if (!refreshRates.Contains(refreshRate)) refreshRates.Add(refreshRate);
        }

        refreshRates.Sort();
        List<string> options = refreshRates.ConvertAll(rate => rate + " Hz");
        RefreshRateDropdown.ClearOptions();
        RefreshRateDropdown.AddOptions(options);
    }
}
