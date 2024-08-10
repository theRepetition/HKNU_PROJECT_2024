using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    public GameObject inventoryPanel; // �κ��丮 �г�
    public GameObject ammoInventoryPanel; // ź�� �κ��丮 �г�
    public GameObject ammoButtonPrefab; // ź�� ��ư Prefab


    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private InventorySlot[] slots; // �κ��丮 ���Ե�
    private Inventory inventory; // �κ��丮 �ν��Ͻ�

    private List<AmmoButton> ammoButtons = new List<AmmoButton>();

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
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.R))
        {
            ToggleInventory();
        }
    }
    public void ToggleInventory()
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);
        ammoInventoryPanel.SetActive(isActive);

        if (isActive)
        {
            if (playerMovement != null) playerMovement.DisableMovement();
            if (playerCombat != null) playerCombat.enabled = false;

            UpdateAmmoButtons();
        }
        else
        {
            if (playerMovement != null) playerMovement.EnableMovement();
            if (playerCombat != null) playerCombat.enabled = true;
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
        if (AmmoManager.Instance == null)
        {
            Debug.LogError("AmmoManager instance is null!");
            return;
        }
        // ź�� �г� �ʱ�ȭ
        foreach (Transform child in ammoInventoryPanel.transform)
        {
            Destroy(child.gameObject); // ��� �ڽ� ������Ʈ ����
        }
        ammoButtons.Clear();

        List<Ammo> ammoTypes = AmmoManager.Instance.GetAmmoList();
        foreach (Ammo ammo in ammoTypes)
        {
            Debug.Log($"Creating button for: {ammo.itemName}, Quantity: {ammo.quantity}");
            GameObject ammoButtonObject = Instantiate(ammoButtonPrefab, ammoInventoryPanel.transform);
            AmmoButton ammoButton = ammoButtonObject.GetComponent<AmmoButton>();
            ammoButton.SetAmmo(ammo);
            ammoButtons.Add(ammoButton);
        }
    }



}
