using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Localization;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private Image slotImage;
    
    [SerializeField]
    private TextMeshProUGUI itemNumText;

    [Tooltip("현재 슬롯에 아이템이 있는지 판단하는 bool 변수입니다. 기본적으로 Flashlight를 제외한 슬롯은 false여야 합니다.")]
    [SerializeField] private bool m_bHasItem;
    public bool HasItem => m_bHasItem;

    public LocalizedString ItemName { get; private set; }
    public LocalizedString ItemDescription { get; private set; }

    public void UpdateSlot(ConsumableItemSO item, int itemNum)
    {
        Debug.Log("UpdateSlot called with item: " + item + ", itemNum: " + itemNum);
        
        if (itemNum != 0)
        {
            // Debug.Log(item.Icon == null ? "item.Icon is null" : "item.Icon is OK");
            
            m_bHasItem = true;
            
            slotImage.sprite = item.Icon;
            SetAlpha(slotImage, 1f);
            itemNumText.text = itemNum.ToString();

            ItemName = item.ItemName;
            ItemDescription = item.Description;
        }
        else
        {
            m_bHasItem = false;
            
            slotImage.sprite = null;
            SetAlpha(slotImage, 0 / 255f);
            itemNumText.text = "";
        }
    }

    public void SetAlpha(Image targetImage, float alpha)
    {
        Color color = targetImage.color;
        color.a = alpha; // 알파만 바꾸고
        targetImage.color = color; // 다시 적용해
    }
}
