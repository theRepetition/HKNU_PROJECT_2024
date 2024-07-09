using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon; // 아이템 아이콘
    public Button removeButton; // 제거 버튼
    public GameObject infoPanel; // 간략한 정보 패널
    public TextMeshProUGUI infoText; // 간략한 정보 텍스트
    public Item item; // 슬롯에 저장된 아이템

    public void AddItem(Item newItem)
    {
        Debug.Log($"Adding item: {newItem.itemName}");
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
}
