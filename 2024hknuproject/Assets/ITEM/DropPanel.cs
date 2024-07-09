using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPanel : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot slot = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (slot != null)
        {
            Item item = slot.item;
            slot.ClearSlot();
            // �κ��丮���� ������ ����
            Inventory.instance.Remove(item);
            // �������� ���� ���忡 ���� (������ �ܼ��� �ֿܼ� ���)
            Debug.Log(item.itemName + " �������� ���忡 ����߽��ϴ�.");
        }
    }
}
