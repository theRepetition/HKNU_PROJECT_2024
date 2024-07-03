using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public GameObject inventoryPanel; // �κ��丮 �г�
    public Text itemInfoText; // ������ ���� �ؽ�Ʈ
    private InventorySlot[] slots; // �κ��丮 ���Ե�
    private Inventory inventory; // �κ��丮 �ν��Ͻ�

    void Start()
    {
        slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventoryPanel.SetActive(false); // ������ �� �κ��丮 �г��� ��Ȱ��ȭ
    }

    void Update()
    {
        // 'I' Ű�� ���� �κ��丮 �г��� Ȱ��ȭ ���¸� ���
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("IŰ ����.");
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
