using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CustomDropdown : MonoBehaviour
{
    [System.Serializable]
    public class DropdownItem
    {
        public Button button;         // 항목 버튼
        public TMP_Text label;        // 항목 텍스트
    }

    public TMP_Text selectedLabel;          // 현재 선택된 텍스트
    public GameObject itemContainer;        // 항목들을 담는 부모 오브젝트
    public List<DropdownItem> items = new List<DropdownItem>();
    public GamePlaySettingsUIPresenter gamePlaySettingsUIPresenter;

    private bool isOpen = false;

    void OnEnable()
    {
        // 초기에는 항목 숨김
        itemContainer.SetActive(false);

        // 각 항목 버튼 클릭 이벤트 연결
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            if (items[index].button != null)
                items[index].button.onClick.AddListener(() => OnItemClicked(index));
        }

        //현재 언어 불러오기
        selectedLabel.text = items[gamePlaySettingsUIPresenter.m_gamePlaySettingsSO.LanguageLocaleIndex].label.text;
    }

    public void DropdownOpen()
    {
        itemContainer.SetActive(true);
    }

    public void DropdownClose()
    {
        itemContainer.SetActive(false);
    }

    private void OnItemClicked(int index)
    {
        RefreshSelectedLabel(index);
        //드롭다운 선택시 이루어질 기능
        gamePlaySettingsUIPresenter.OnLanguageChanged(index);
        DropdownClose(); // 선택 후 닫기
    }

    private void RefreshSelectedLabel(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            selectedLabel.text = items[index].label.text;
        }
    }
}
