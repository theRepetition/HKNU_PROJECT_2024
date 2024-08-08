using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoButton : MonoBehaviour
{
    public TextMeshProUGUI ammoNameText;
    public TextMeshProUGUI ammoQuantityText;
    private Ammo ammo;

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
        if (ammo != null && ammo.quantity > 0)
        {
            PlayerCombat playerCombat = FindObjectOfType<PlayerCombat>();
            if (playerCombat != null)
            {
                playerCombat.LoadAmmo(ammo);
                ammo.quantity--; // 탄약 수량 감소
                UpdateUI(); // UI 업데이트
            }
        }
    }
}
