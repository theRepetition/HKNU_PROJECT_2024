using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AmmoButton : MonoBehaviour
{
    public TMP_Text ammoNameText; // 탄약 이름 텍스트
    public TMP_Text ammoQuantityText; // 탄약 수량 텍스트
    public Image ammoIcon; // 탄약 아이콘 이미지

    private Ammo ammo;

    public void SetAmmo(Ammo ammo)
    {
        if (ammo == null)
        {
            Debug.LogError("Ammo is null in SetAmmo!");
            return;
        }

        this.ammo = ammo;
        ammoNameText.text = ammo.itemName; // 수정된 부분
        ammoQuantityText.text = ammo.quantity.ToString();
        ammoIcon.sprite = ammo.icon;
    }

    public void OnClick()
    {
        // 장전 로직 추가
        Debug.Log($"Ammo {ammo.itemName} selected with {ammo.quantity} bullets remaining."); // 수정된 부분
    }
}
