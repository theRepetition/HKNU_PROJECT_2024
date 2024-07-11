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
    public GameObject infoPanel; // 간략한 정보 패널
    public TextMeshProUGUI infoText; // 간략한 정보 텍스트
    public Item item; // 슬롯에 저장된 아이템

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
        Inventory.instance.Remove(item);
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
        if (item != null)
        {
            infoPanel.SetActive(true);
            infoText.text = $"{item.itemName}\n{item.description}";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.SetActive(false);
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
