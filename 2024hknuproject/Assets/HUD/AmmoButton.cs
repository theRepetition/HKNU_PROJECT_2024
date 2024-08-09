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
                ammoChamberUI.LoadAmmo(ammo); // AmmoChamberUI�� ź���� �ε�
                ammo.quantity--; // ź�� ���� ����
                UpdateUI(); // UI ������Ʈ
            }
            else
            {
                Debug.LogError("AmmoChamberUI�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("ź���� ���ų� ź�� ������ �����մϴ�.");
        }
    }



}
