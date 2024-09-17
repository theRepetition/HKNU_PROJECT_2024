using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public Ammo ammo;

    public void PickUp()
    {
        Debug.Log(ammo.itemName);
        AmmoManager.Instance.AddAmmo(ammo);
        InventoryUIManager.Instance.UpdateAmmoButtons(); // UI
        Destroy(gameObject);
    }
}
