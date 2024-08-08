using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AmmoButton : MonoBehaviour
{
    public TMP_Text ammoNameText; // ź�� �̸� �ؽ�Ʈ
    public TMP_Text ammoQuantityText; // ź�� ���� �ؽ�Ʈ
    public Image ammoIcon; // ź�� ������ �̹���

    private Ammo ammo;

    public void SetAmmo(Ammo ammo)
    {
        if (ammo == null)
        {
            Debug.LogError("Ammo is null in SetAmmo!");
            return;
        }

        this.ammo = ammo;
        ammoNameText.text = ammo.itemName; // ������ �κ�
        ammoQuantityText.text = ammo.quantity.ToString();
        ammoIcon.sprite = ammo.icon;
    }

    public void OnClick()
    {
        // ���� ���� �߰�
        Debug.Log($"Ammo {ammo.itemName} selected with {ammo.quantity} bullets remaining."); // ������ �κ�
    }
}
