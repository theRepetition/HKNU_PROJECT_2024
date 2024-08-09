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
        Debug.Log("Ammo button clicked!");

        if (ammo != null && ammo.quantity > 0)
        {
            AmmoChamberUI ammoChamberUI = FindObjectOfType<AmmoChamberUI>();
            if (ammoChamberUI != null)
            {
                ammoChamberUI.LoadAmmo(ammo); // AmmoChamberUI에 탄약을 로드
                ammo.quantity--; // 탄약 수량 감소
                UpdateUI(); // UI 업데이트
            }
            else
            {
                Debug.LogError("AmmoChamberUI를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("탄약이 없거나 탄약 수량이 부족합니다.");
        }
    }



}
