using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    public GameObject inventoryPanel; // 인벤토리 패널
    public GameObject ammoInventoryPanel; // 탄약 인벤토리 패널
    public GameObject ammoButtonPrefab; // 탄약 버튼 Prefab


    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private InventorySlot[] slots; // 인벤토리 슬롯들
    private Inventory inventory; // 인벤토리 인스턴스

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
        inventoryPanel.SetActive(false); // 시작할 때 인벤토리 패널을 비활성화
        ammoInventoryPanel.SetActive(false); // 시작할 때 탄약 인벤토리 패널 비활성화

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
        // 탄약 패널 초기화
        foreach (Transform child in ammoInventoryPanel.transform)
        {
            Destroy(child.gameObject); // 모든 자식 오브젝트 제거
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
