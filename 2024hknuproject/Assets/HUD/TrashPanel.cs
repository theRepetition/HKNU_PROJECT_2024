using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashPanel : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var slot = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (slot != null)
        {
            Inventory.instance.Remove(slot.GetItem());
            slot.ClearSlot();
        }
    }
}
