using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    public GameObject inventoryPanel; // �κ��丮 �г�
    public GameObject ammoInventoryPanel; // ź�� �κ��丮 �г�

    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private InventorySlot[] slots; // �κ��丮 ���Ե�
    private Inventory inventory; // �κ��丮 �ν��Ͻ�

    // ź�� ��ư��
    public AmmoButton[] ammoButtons;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventoryPanel.SetActive(false); // ������ �� �κ��丮 �г��� ��Ȱ��ȭ
        ammoInventoryPanel.SetActive(false); // ������ �� ź�� �κ��丮 �г� ��Ȱ��ȭ

        playerMovement = FindObjectOfType<PlayerMovement>();
        playerCombat = FindObjectOfType<PlayerCombat>();
    }

    void Update()
    {
        // 'I' Ű�� 'R' Ű�� ���� �κ��丮 �г��� Ȱ��ȭ ���¸� ���
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.R))
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);
            ammoInventoryPanel.SetActive(isActive);

            if (isActive)
            {
                // �κ��丮 Ȱ��ȭ ��
                if (playerMovement != null) playerMovement.DisableMovement(); // �κ��丮 ���� ����
                if (playerCombat != null) playerCombat.enabled = false;

                // ź�� ��ư ������Ʈ
                UpdateAmmoButtons();
            }
            else
            {
                // �κ��丮 ��Ȱ��ȭ ��
                if (playerMovement != null) playerMovement.EnableMovement(); // �ٽ� ������ Ȱ��ȭ
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

    public void UpdateAmmoButtons()
    {
        if (ammoButtons == null || ammoButtons.Length == 0)
        {
            Debug.LogError("Ammo buttons are not set in the inspector!");
            return;
        }

        Ammo[] ammoTypes = AmmoManager.Instance.GetAmmoList().ToArray(); // ź�� ���� ��������
        if (ammoTypes == null)
        {
            Debug.LogError("Ammo list is null!");
            return;
        }

        for (int i = 0; i < ammoButtons.Length; i++)
        {
            if (i < ammoTypes.Length)
            {
                if (ammoButtons[i] != null)
                {
                    ammoButtons[i].SetAmmo(ammoTypes[i]);
                }
                else
                {
                    Debug.LogError($"Ammo button at index {i} is null!");
                }
            }
        }
    }
}
