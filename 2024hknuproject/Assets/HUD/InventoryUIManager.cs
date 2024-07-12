using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public GameObject inventoryPanel; // �κ��丮 �г�
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private InventorySlot[] slots; // �κ��丮 ���Ե�
    private Inventory inventory; // �κ��丮 �ν��Ͻ�

    void Start()
    {
        slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventoryPanel.SetActive(false); // ������ �� �κ��丮 �г��� ��Ȱ��ȭ
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerCombat = FindObjectOfType<PlayerCombat>();
    }

    void Update()
    {
        // 'I' Ű�� ���� �κ��丮 �г��� Ȱ��ȭ ���¸� ���
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);

            if (isActive)
            {
                // �κ��丮 Ȱ��ȭ ��

                if (playerMovement != null) playerMovement.DisableMovement(); //�κ��丮 ���� ����
                if (playerCombat != null) playerCombat.enabled = false;
            }
            else
            {
                // �κ��丮 ��Ȱ��ȭ ��

                if (playerMovement != null) playerMovement.EnableMovement(); //�ٽ� ������ Ȱ��ȭ
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
