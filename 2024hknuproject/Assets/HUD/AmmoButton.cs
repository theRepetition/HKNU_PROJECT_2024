using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoButton : MonoBehaviour
{
    public TextMeshProUGUI ammoNameText;
    public TextMeshProUGUI ammoQuantityText;
    private Ammo ammo;
    private AmmoChamberUI ACU;

    void Start()
    {
        ACU = FindObjectOfType<AmmoChamberUI>(); // PlayerTurnManager를 찾아서 할당
        
    }
    public void SetAmmo(Ammo newAmmo)
    {
        ammo = newAmmo;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ammo != null)
        {
            ammoNameText.text = ammo.itemName;
            ammoQuantityText.text = ammo.quantity.ToString();
        }
    }

    public void OnAmmoButtonClicked()
    {
        Debug.Log("Ammo button clicked!");

        if (ammo != null && ammo.quantity > 0 && ACU.Ammoisfull()<6)
        {
            AmmoChamberUI ammoChamberUI = FindObjectOfType<AmmoChamberUI>();
            if (ammoChamberUI != null)
            {
                ammoChamberUI.LoadAmmo(ammo); // 
                ammo.quantity--; // 
                UpdateUI(); //
            }
            else
            {
                
            }
        }
        else
        {
           
        }
    }



}
