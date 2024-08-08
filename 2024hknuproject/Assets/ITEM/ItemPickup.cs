using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // 이 오브젝트에 연결된 아이템

    public void PickUp()
    {
        Debug.Log("아이템 주움: " + item.itemName);
        bool wasPickedUp = false;

        if (item.itemType == Item.ItemType.Ammo)
        {
            AmmoManager.Instance.AddAmmo((Ammo)item); // 아이템을 Ammo로 캐스팅하여 추가
            wasPickedUp = true;
        }
        else
        {
            wasPickedUp = Inventory.instance.Add(item);
        }

        if (wasPickedUp)
        {
            Destroy(gameObject); // 아이템 오브젝트 제거
        }
    }
}
