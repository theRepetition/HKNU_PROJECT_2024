using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; private set; }

    public List<Ammo> ammoTypes = new List<Ammo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddAmmo(Ammo ammo)
    {
        Ammo existingAmmo = ammoTypes.Find(a => a.itemName == ammo.itemName); 
        if (existingAmmo != null)
        {
            existingAmmo.quantity += ammo.quantity;
        }
        else
        {
            ammoTypes.Add(ammo);
        }
    }
    

    public List<Ammo> GetAmmoList()
    {
        return ammoTypes;
    }
}
