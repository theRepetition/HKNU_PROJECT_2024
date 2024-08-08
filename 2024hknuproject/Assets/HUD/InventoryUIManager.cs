using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; } // 싱글톤 인스턴스

    public GameObject inventoryPanel; // 인벤토리 패널
    public GameObject ammoInventoryPanel; // 탄약 인벤토리 패널

    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private InventorySlot[] slots; // 인벤토리 슬롯들
    private Inventory inventory; // 인벤토리 인스턴스

    // 탄약 버튼들
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
        inventoryPanel.SetActive(false); // 시작할 때 인벤토리 패널을 비활성화
        ammoInventoryPanel.SetActive(false); // 시작할 때 탄약 인벤토리 패널 비활성화

        playerMovement = FindObjectOfType<PlayerMovement>();
        playerCombat = FindObjectOfType<PlayerCombat>();
    }

    void Update()
    {
        // 'I' 키나 'R' 키를 눌러 인벤토리 패널의 활성화 상태를 토글
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.R))
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);
            ammoInventoryPanel.SetActive(isActive);

            if (isActive)
            {
                // 인벤토리 활성화 시
                if (playerMovement != null) playerMovement.DisableMovement(); // 인벤토리 열면 정지
                if (playerCombat != null) playerCombat.enabled = false;

                // 탄약 버튼 업데이트
                UpdateAmmoButtons();
            }
            else
            {
                // 인벤토리 비활성화 시
                if (playerMovement != null) playerMovement.EnableMovement(); // 다시 움직임 활성화
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

        Ammo[] ammoTypes = AmmoManager.Instance.GetAmmoList().ToArray(); // 탄약 종류 가져오기
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
