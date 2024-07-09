using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // �� ������Ʈ�� ����� ������

    public void PickUp()
    {
        Debug.Log("������ �ֿ�: " + item.itemName);
        bool wasPickedUp = Inventory.instance.Add(item);

        if (wasPickedUp)
        {
            Destroy(gameObject); // ������ ������Ʈ ����
        }
    }
}
