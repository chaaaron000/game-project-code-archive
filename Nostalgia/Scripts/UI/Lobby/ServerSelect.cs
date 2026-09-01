using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System.Threading;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using Fusion.Photon.Realtime;
using System;
using Nostal.Network;

public class ServerSelect : MonoBehaviour
{
    public TMP_Dropdown dropdown; // 드롭다운 UI 컴포넌트
    public CanvasGroup group;
    public NetworkManager networkManager; // 네트워크 매니저
    public void Awake() {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        //GetBestRegionCodeAsync(); // 비동기 메소드 호출
        GameObject.Find("NetworkManager")?.TryGetComponent(out networkManager);
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not found in the scene!");
        }
        else {
            networkManager.RefreshServerSelect(this);
        }
        
    }

    public async Task<bool> FindBestRegionAsync()
    {
        string region = await GetBestRegionCodeAsync("kr");
        // 옵션에서 region 코드와 일치하는 인덱스 찾기
        int index = dropdown.options.FindIndex(o => 
            o != null && string.Equals(
                o.text.Split('=')[0].Trim(), region, StringComparison.OrdinalIgnoreCase));

        if (index < 0)
        {
            Debug.LogWarning($"Region '{region}' not found in dropdown");
            return false;
        }

        // 이벤트를 태우고 싶으면 SetValue 사용, 이벤트 없이 값만 바꾸려면 SetValueWithoutNotify
        StartCoroutine(ShowText()); // 텍스트 표시 코루틴 시작
        dropdown.value = index;          // onValueChanged 발생
        dropdown.RefreshShownValue();
        return true;
    }

    public IEnumerator ShowText() {
        float duration = 3f;
        float t = 0f;
        while (t < duration) {
            t += Time.deltaTime;
            group.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    public async Task<string> GetBestRegionCodeAsync(string fallback = "kr")
    {
        var regions = await NetworkRunner.GetAvailableRegions();

        if (regions == null || regions.Count == 0)
            return fallback;

        // 핑 기준 최저 지역 선택
        var best = regions.OrderBy(r => r.RegionPing).First();
        Debug.Log($"Best region: {best.RegionCode} ({best.RegionPing} ms)");
        return best.RegionCode; // 예: "kr", "jp", "asia" 등
    }

    public void SetNetworkManager(NetworkManager manager)
    {
        networkManager = manager;
    }

    public void OnDropdownValueChanged(int index)
    {
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager is not set!");
            return;
        }

        // 선택된 지역 코드 추출
        string selectedRegion = dropdown.options[index].text.Split('=')[0].Trim();
        networkManager.SetRegion(selectedRegion);
        Debug.Log($"Selected region: {selectedRegion}");
    }
}
