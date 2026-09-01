using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    public InventoryUI inventoryUI;
    private ConsumableItemSO selectedItem;
    public Player player;
    
    private List<ConsumableItemSO> items = new List<ConsumableItemSO>{null, null, null, null, null};
    private List<int> itemNums = new List<int>{0,0,0,0,0};
    private int itemCount = 0;
    private int maxItemCount = 5; //인벤토리 최대 아이템 수

    public bool IsItemSelected { get; private set; } = false;

    public void Awake() {
        player = GetOwnerPlayer();
    }

    //키보드 1,2,3 누를 시 호출
    public void OnSelectItem(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        
        Debug.Log("OnSelectItem 호출됨");

        if (player.CanUseItem() == false)
        {
            return;
        }

        // 어떤 바인딩 이름인지 가져오기
        int bindingIndex = context.action.GetBindingIndexForControl(context.control);
        string bindingName = context.action.bindings[bindingIndex].name;

        Debug.Log("bindingName: " + bindingName);

        switch (bindingName)
        {
            case "item1":
                SelectItem(0);
                break;
            case "item2":
                SelectItem(1);
                break;
            case "item3":
                SelectItem(2);
                break;
            case "item4":
                SelectItem(3);
                break;
            case "item5":
                SelectItem(4);
                break;
            default:
                Debug.Log("알 수 없는 바인딩: " + bindingName);
                break;
        }
    }

    //flashlight Toggle이 변할 때 호출
    public void OnFlashlightChange(bool flashlightToggle)
    {
        if (flashlightToggle == true)  // 플래시라이트가 켜질 때
        {
            selectedItem = null;
            inventoryUI.SelectItem(5);
            IsItemSelected = false;
        }
        else  // 플래시라이트가 꺼질 때, 순서 보장이 안되므로 아이템 선택 없을 때만 호출
        {
            if (selectedItem == null)
            {
                inventoryUI.SelectItem(-1);
            }
        }
    }

    public void SelectItem(int index)
    {
        int selectedIndex;

        // 이미 선택된 아이템일 경우 선택 취소
        if (selectedItem != null && selectedItem == items[index])
        {
            selectedItem = null;
            selectedIndex = -1;
            IsItemSelected = false;
        }
        else
        {
            //칸이 비어있을 경우 return
            if (items[index] == null)
            {
                return;
            }

            // 아닐 경우 선택된 아이템을 변경
            selectedItem = items[index];
            selectedIndex = index;
            IsItemSelected = true;
            
            // flashlight가 켜져있을 경우 flashlight를 끔
            if (player._playerFlashlight.FlashlightToggle)
            {
                player._playerFlashlight.FlashlightToggle = false;
            }
        }

        inventoryUI.SelectItem(selectedIndex);
    }

    //마우스 좌클릭 시 호출
    public void OnUseItem(InputAction.CallbackContext context)
    {
        //선택된 아이템이 없을 때 / 아이템 사용 불가 상태일 때 return
        if (selectedItem == null || player.CanUseItem() == false)
        {
            return;
        }

        //아이템 사용
        selectedItem.Use(GetOwnerPlayer());
        RemoveItem(selectedItem);

        selectedItem = null;
        inventoryUI.SelectItem(-1);
        IsItemSelected = false;
    }

    //아이템 획득 시도시 먹을 수 있는지 확인
    public bool CanAddItem(ConsumableItemSO item)
    {
        if (itemCount >= maxItemCount && !items.Contains(item))
        {
            StartCoroutine(CantAddItem());
            return false; //인벤토리가 가득 차있지만, 새로운 아이템이면 false
        }
        
        return true;
    }

    public IEnumerator CantAddItem()
    {
        if (UIManager.Instance.CantAddItemUIController == null)
        {
            UIManager.Instance.EnableCantAddItemUI();
        }

        UIManager.Instance.CantAddItemUIController.Show();
        yield return new WaitForSeconds(3f);
        UIManager.Instance.CantAddItemUIController.Hide();
    }

    //아이템 추가 <- 빈자리 확인 하는 로직 필요
    public void AddItem(ConsumableItemSO item) {
        int index = -1;
        if(items.Contains(item)) {
            index = items.IndexOf(item);
            itemNums[index]++;
        }
        else {
            for(int i=0; i<items.Count; i++) {
                if(items[i] == null) {
                    items[i] = item;
                    itemNums[i] = 1;
                    index = i; //빈자리에 추가된 아이템의 인덱스
                    itemCount++;
                    break;
                }
            }
        }

        if(index == -1) {
            Debug.LogError("인벤토리가 가득 찼습니다. 아이템을 추가할 수 없습니다.");
            return;
        }
        //인벤토리 UI 업데이트
        inventoryUI.UpdateSlots(index, item, itemNums[index]);
    }

    //아이템 사용 후 제거
    public void RemoveItem(ConsumableItemSO item) {
        int index;
        if(items.Contains(item)) {
            index = items.IndexOf(item);
            itemNums[index]--;
            if(itemNums[index] <= 0) {
                items[index] = null;
                itemNums[index] = 0;
                itemCount--;
            }
        }
        else {
            Debug.LogError("인벤토리에 없는 아이템을 제거 시도했습니다: " + item.ItemName);
            return;
        }

        //인벤토리 UI 업데이트
        inventoryUI.UpdateSlots(index, item, itemNums[index]);
    }

    public void ClearInventory() {
        for(int i=0; i<items.Count; i++) {
            int itemNum = itemNums[i];
            for(int j=0; j<itemNum; j++) {
                RemoveItem(items[i]);
            }
        }
    }

    public Player GetOwnerPlayer()
    {
        Player player = gameObject.GetComponent<Player>();

        if (player != null)
        {
            return player;
        }
        else
        {
            Debug.LogError("Player 컴포넌트를 찾을 수 없습니다.");
            return null;
        }
    }
}
