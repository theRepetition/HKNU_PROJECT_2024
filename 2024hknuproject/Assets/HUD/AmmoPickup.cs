using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public Ammo ammo;

    public void PickUp()
    {
        Debug.Log("ź�� �ֿ�: " + ammo.itemName);
        AmmoManager.Instance.AddAmmo(ammo);
        InventoryUIManager.Instance.UpdateAmmoButtons();
        Destroy(gameObject); // ź�� ������Ʈ ����
    }
}
