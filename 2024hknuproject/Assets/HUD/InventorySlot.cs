using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public Image icon; // ������ ������
    public Button removeButton; // ���� ��ư
    public GameObject infoPanel; // ������ ���� �г�
    public TextMeshProUGUI infoText; // ������ ���� �ؽ�Ʈ
    public Item item; // ���Կ� ����� ������

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
        ClearSlot(); // ������ ���� �ڵ� �߰�
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

    // ������ ��ȯ �޼��� �߰�
    public Item GetItem()
    {
        return item;
    }

    // ������ ���� �޼��� �߰�
    public void SetItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item != null ? item.icon : null;
        icon.enabled = item != null;
        removeButton.interactable = item != null;
    }

    // IDropHandler �������̽� ����
    public void OnDrop(PointerEventData eventData)
    {
        var droppedItem = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (droppedItem != null && droppedItem != this)
        {
            SwapItems(droppedItem); // ������ ��ȯ
        }
    }

    private void SwapItems(InventorySlot droppedItem)
    {
        // ���� �� ������ ��ȯ
        var tempItem = droppedItem.GetItem();
        droppedItem.SetItem(GetItem());
        SetItem(tempItem);
    }
}
