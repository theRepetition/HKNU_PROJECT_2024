using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // �� ������Ʈ�� ����� ������

    public void PickUp()
    {
        Debug.Log("������ �ֿ�: " + item.itemName);
        bool wasPickedUp = false;

        if (item.itemType == Item.ItemType.Ammo)
        {
            AmmoManager.Instance.AddAmmo((Ammo)item); // �������� Ammo�� ĳ�����Ͽ� �߰�
            wasPickedUp = true;
        }
        else
        {
            wasPickedUp = Inventory.instance.Add(item);
        }

        if (wasPickedUp)
        {
            Destroy(gameObject); // ������ ������Ʈ ����
        }
    }
}
