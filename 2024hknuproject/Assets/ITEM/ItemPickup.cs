using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // 이 오브젝트에 연결된 아이템

    public void PickUp()
    {
        Debug.Log("아이템 주움: " + item.itemName);
        bool wasPickedUp = Inventory.instance.Add(item);

        if (wasPickedUp)
        {
            Destroy(gameObject); // 아이템 오브젝트 제거
        }
    }
}
