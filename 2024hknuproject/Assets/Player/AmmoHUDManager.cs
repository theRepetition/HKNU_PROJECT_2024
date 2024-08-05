using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoHUDManager : MonoBehaviour
{
    public GameObject ammoInventoryUI; // ź�� �κ��丮 UI
    public GameObject ammoButtonPrefab; // ź�� ��ư ������
    public Transform ammoButtonParent; // ź�� ��ư���� �� �θ� ��ü

    private PlayerCombat playerCombat;

    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        UpdateAmmoInventoryUI();
    }

    // ź�� �κ��丮 UI ������Ʈ
    public void UpdateAmmoInventoryUI()
    {
        // ���� ��ư ����
        foreach (Transform child in ammoButtonParent)
        {
            Destroy(child.gameObject);
        }

        // ���ο� ��ư ����
        foreach (var ammo in playerCombat.ammoInventory)
        {
            GameObject ammoButton = Instantiate(ammoButtonPrefab, ammoButtonParent);
            ammoButton.GetComponent<AmmoButton>().ammo = ammo;
            ammoButton.GetComponentInChildren<Text>().text = $"{ammo.name}\n{ammo.damage} dmg\n{ammo.effect}";
        }
    }

    // ź�� �κ��丮 UI ���
    public void ToggleAmmoInventoryUI()
    {
        ammoInventoryUI.SetActive(!ammoInventoryUI.activeSelf);
    }
}
