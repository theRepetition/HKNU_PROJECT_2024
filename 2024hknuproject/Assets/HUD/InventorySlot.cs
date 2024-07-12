using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public Image icon; // 아이템 아이콘
    public Button removeButton; // 제거 버튼
    public Item item; // 슬롯에 저장된 아이템

    private void Awake()
    {
        // 필요한 UI 요소가 할당되었는지 확인
        if (icon == null || removeButton == null)
        {
            Debug.LogError("UI 요소가 할당되지 않았습니다.");
        }
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton()
    {
        int slotIndex = transform.GetSiblingIndex(); // 슬롯의 인덱스 가져오기
        Inventory.instance.RemoveAt(slotIndex); // 해당 인덱스의 아이템 제거
        ClearSlot(); // 슬롯을 비우는 코드 추가
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 이 메서드는 이제 빈 상태로 남깁니다.
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 이 메서드는 이제 빈 상태로 남깁니다.
    }

    // 아이템 반환 메서드 추가
    public Item GetItem()
    {
        return item;
    }

    // 아이템 설정 메서드 추가
    public void SetItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item != null ? item.icon : null;
        icon.enabled = item != null;
        removeButton.interactable = item != null;
    }

    // IDropHandler 인터페이스 구현
    public void OnDrop(PointerEventData eventData)
    {
        var droppedItem = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (droppedItem != null && droppedItem != this)
        {
            SwapItems(droppedItem); // 아이템 교환
        }
    }

    private void SwapItems(InventorySlot droppedItem)
    {
        // 슬롯 간 아이템 교환
        var tempItem = droppedItem.GetItem();
        droppedItem.SetItem(GetItem());
        SetItem(tempItem);
    }
}
