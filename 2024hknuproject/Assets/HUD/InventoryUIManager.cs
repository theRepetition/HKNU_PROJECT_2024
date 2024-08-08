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

    public Button reloadButton; // 장전 완료 버튼
    public Button rollbackButton; // 초기화 버튼

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

        // 장전 완료 및 초기화 버튼 이벤트 등록
        reloadButton.onClick.AddListener(OnReloadButtonClicked);
        rollbackButton.onClick.AddListener(OnRollbackButtonClicked);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.R))
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
        foreach (Transform child in ammoInventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }
        ammoButtons.Clear();

        List<Ammo> ammoTypes = AmmoManager.Instance.GetAmmoList();
        foreach (Ammo ammo in ammoTypes)
        {
            GameObject ammoButtonObject = Instantiate(ammoButtonPrefab, ammoInventoryPanel.transform);
            AmmoButton ammoButton = ammoButtonObject.GetComponent<AmmoButton>();
            ammoButton.SetAmmo(ammo);
            ammoButtons.Add(ammoButton);
        }
    }

    private void OnReloadButtonClicked()
    {
        // 장전 완료 로직
        Debug.Log("Reload button clicked. Confirming ammo load.");
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            TurnManager.Instance.NextTurn();
        }
    }

    private void OnRollbackButtonClicked()
    {
        // 초기화 로직
        Debug.Log("Rollback button clicked. Resetting ammo load.");
        // 필요한 초기화 작업을 추가
    }
}
