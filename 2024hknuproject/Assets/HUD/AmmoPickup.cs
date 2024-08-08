using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public Ammo ammo;

    public void PickUp()
    {
        Debug.Log("탄약 주움: " + ammo.itemName);
        AmmoManager.Instance.AddAmmo(ammo);
        InventoryUIManager.Instance.UpdateAmmoButtons();
        Destroy(gameObject); // 탄약 오브젝트 제거
    }
}
