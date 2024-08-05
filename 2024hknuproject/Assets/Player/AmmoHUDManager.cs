using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoHUDManager : MonoBehaviour
{
    public GameObject ammoInventoryUI; // 탄약 인벤토리 UI
    public GameObject ammoButtonPrefab; // 탄약 버튼 프리팹
    public Transform ammoButtonParent; // 탄약 버튼들이 들어갈 부모 객체

    private PlayerCombat playerCombat;

    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        UpdateAmmoInventoryUI();
    }

    // 탄약 인벤토리 UI 업데이트
    public void UpdateAmmoInventoryUI()
    {
        // 기존 버튼 제거
        foreach (Transform child in ammoButtonParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 버튼 생성
        foreach (var ammo in playerCombat.ammoInventory)
        {
            GameObject ammoButton = Instantiate(ammoButtonPrefab, ammoButtonParent);
            ammoButton.GetComponent<AmmoButton>().ammo = ammo;
            ammoButton.GetComponentInChildren<Text>().text = $"{ammo.name}\n{ammo.damage} dmg\n{ammo.effect}";
        }
    }

    // 탄약 인벤토리 UI 토글
    public void ToggleAmmoInventoryUI()
    {
        ammoInventoryUI.SetActive(!ammoInventoryUI.activeSelf);
    }
}
