using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public GameObject inventoryPanel; // 인벤토리 패널
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private InventorySlot[] slots; // 인벤토리 슬롯들
    private Inventory inventory; // 인벤토리 인스턴스

    void Start()
    {
        slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventoryPanel.SetActive(false); // 시작할 때 인벤토리 패널을 비활성화
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerCombat = FindObjectOfType<PlayerCombat>();
    }

    void Update()
    {
        // 'I' 키를 눌러 인벤토리 패널의 활성화 상태를 토글
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);

            if (isActive)
            {
                // 인벤토리 활성화 시

                if (playerMovement != null) playerMovement.DisableMovement(); //인벤토리 열면 정지
                if (playerCombat != null) playerCombat.enabled = false;
            }
            else
            {
                // 인벤토리 비활성화 시

                if (playerMovement != null) playerMovement.EnableMovement(); //다시 움직임 활성화
                if (playerCombat != null) playerCombat.enabled = true;
            }
        }
    }

    public void UpdateUI()
    {
        List<Item> items = inventory.GetItems();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                slots[i].AddItem(items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
