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
            // 인벤토리에서 아이템 제거
            Inventory.instance.Remove(item);
            // 아이템을 게임 월드에 생성 (아직은 단순히 콘솔에 출력)
            Debug.Log(item.itemName + " 아이템을 월드에 드롭했습니다.");
        }
    }
}
