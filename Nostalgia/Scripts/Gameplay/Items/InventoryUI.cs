using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    //선택한 아이템의 아이콘을 나타내는 프레임
    [SerializeField]
    private Image selectedIcon;
    
    [SerializeField]
    private List<Slot> slots;

    [Header("아이템 정보 UI 텍스트")] 
    [SerializeField] private TMP_Text m_itemNameUI;
    [SerializeField] private TMP_Text m_itemDescriptionUI;

    [Header("아이템 정보 로컬라이즈 컴포넌트")]
    [SerializeField] private LocalizeStringEvent m_itemNameLocalStr;
    [SerializeField] private LocalizeStringEvent m_itemDescriptionLocalStr;

    [Header("랜턴 이름 로컬라이즈 문자열")]
    [SerializeField] private LocalizedString m_flashlightName;

    [Header("빈 로컬라이즈 문자열")] 
    [SerializeField] private LocalizedString m_emptyString;
    [SerializeField] private Canvas m_canvas;

    private void Awake()
    {
        // 빈 문자열로 초기화
        m_itemNameLocalStr.StringReference = m_emptyString;
        m_itemDescriptionLocalStr.StringReference = m_emptyString;
        RefreshLocalizedString();
    }

    public void Show() {
        if(m_canvas != null)
            m_canvas.enabled = true;
    }

    public void Hide() {
        if(m_canvas != null)
            m_canvas.enabled = false;
    }

    public void UpdateSlots(int index, ConsumableItemSO item, int itemNum)
    {
        if (index < 0 || index >= slots.Count)
        {
            Debug.LogError("Invalid index: " + index);
            return;
        }

        //슬롯에 아이템 업데이트
        slots[index].UpdateSlot(item, itemNum);
    }

    /// <summary>
    /// 인벤토리 UI에서 선택한 아이템의 슬롯을 선택합니다. -1을 입력하면 선택 해제됩니다.
    /// </summary>
    /// <param name="index"></param>
    public void SelectItem(int index) 
    {
        if (index < -1 || index >= slots.Count)
        {
            Debug.LogError("Invalid index: " + index);
            return;
        }
        
        //프레임 아이콘 위치 변경
        if (index == -1)
        {
            selectedIcon.transform.position = new Vector3(-1000, -1000, 0); // 화면 밖으로 이동
        }
        else
        {
            selectedIcon.transform.position = slots[index].transform.position;
        }
        
        UpdateItemInformation(index);
    }

    /// <summary>
    /// UI에 아이템 정보를 업데이트합니다.
    /// </summary>
    /// <param name="index">아이템 슬롯의 인덱스</param>
    private void UpdateItemInformation(int index)
    {
        // 아이템 선택 해제
        if (index == -1)
        {
            m_itemNameLocalStr.StringReference = m_emptyString;
            m_itemDescriptionLocalStr.StringReference = m_emptyString;
            RefreshLocalizedString();
            return;
        }

        // 손전등 선택
        if (index == 5)
        {
            m_itemNameLocalStr.StringReference = m_flashlightName;
            m_itemDescriptionLocalStr.StringReference = m_emptyString;
            RefreshLocalizedString();
            return;
        }

        Slot selectedSlot = slots[index];
        if (selectedSlot.HasItem)  // 슬롯에 아이템이 있음
        {
            m_itemNameLocalStr.StringReference = selectedSlot.ItemName;
            m_itemDescriptionLocalStr.StringReference = selectedSlot.ItemDescription;
        }
        else  // 슬롯에 아이템이 없음
        {
            m_itemNameLocalStr.StringReference = m_emptyString;
            m_itemDescriptionLocalStr.StringReference = m_emptyString;
        }

        RefreshLocalizedString();
    }

    private void RefreshLocalizedString()
    {
        m_itemNameLocalStr.RefreshString();
        m_itemDescriptionLocalStr.RefreshString();
    }
}
