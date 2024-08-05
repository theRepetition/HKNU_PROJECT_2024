using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoButton : MonoBehaviour
{
    public Ammo ammo;
    private PlayerCombat playerCombat;

    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (playerCombat != null)
        {
            playerCombat.LoadAmmo(ammo);
        }
    }
}
