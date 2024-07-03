using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public GameObject inventoryPanel; // 인벤토리 패널
    public Text itemInfoText; // 아이템 정보 텍스트
    private InventorySlot[] slots; // 인벤토리 슬롯들
    private Inventory inventory; // 인벤토리 인스턴스

    void Start()
    {
        slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventoryPanel.SetActive(false); // 시작할 때 인벤토리 패널을 비활성화
    }

    void Update()
    {
        // 'I' 키를 눌러 인벤토리 패널의 활성화 상태를 토글
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I키 누름.");
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

    public void UpdateUI()
    {
        List<Item> items = inventory.GetItems();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
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
