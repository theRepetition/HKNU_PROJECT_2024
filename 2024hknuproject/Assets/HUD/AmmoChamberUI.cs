using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AmmoChamberUI : MonoBehaviour
{
    public GameObject chamberSlotPrefab; // 탄환 슬롯 프리팹
    public Transform chamberPanel; // 탄환이 배치될 패널
    public Button reloadButton; // 장전 완료 버튼
    public Button resetButton; // 초기화 버튼

    private List<GameObject> chamberSlots = new List<GameObject>(); // 현재 장전된 탄환 슬롯들
    private List<Ammo> loadedAmmo = new List<Ammo>(); // 현재 장전된 탄약 목록
    private PlayerCombat playerCombat;
    private InventoryUIManager inventoryUIManager;

    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        inventoryUIManager = FindObjectOfType<InventoryUIManager>();
        reloadButton.onClick.AddListener(Reload);
        resetButton.onClick.AddListener(ResetChamber);

        InitializeChamberSlots(6); // 약실에 슬롯 6개 생성 
    }

    // 약실 슬롯 초기화
    void InitializeChamberSlots(int slotCount)
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(chamberSlotPrefab, chamberPanel);
            chamberSlots.Add(slot);
        }
    }
    public void LogChamberSlots()
    {
        Debug.Log("Current Chamber Slots:");
        for (int i = 0; i < chamberSlots.Count; i++)
        {
            var ammo = i < loadedAmmo.Count ? loadedAmmo[i] : null;
            if (ammo != null)
            {
                Debug.Log($"Slot {i + 1}: {ammo.itemName}, Quantity: {ammo.quantity}");
            }
            else
            {
                Debug.Log($"Slot {i + 1}: Empty");
            }
        }
    }

    // 탄약 장전
    public void LoadAmmo(Ammo ammo)
    {
        if (loadedAmmo.Count < chamberSlots.Count)
        {
            loadedAmmo.Add(ammo);
            var slotIndex = loadedAmmo.Count - 1;
            var slotImage = chamberSlots[slotIndex].GetComponent<Image>();

            if (slotImage != null)
            {
                if (ammo.icon != null)
                {
                    slotImage.sprite = ammo.icon;
                    Debug.Log($"Slot {slotIndex} updated with {ammo.itemName} icon.");
                }
                else
                {
                    Debug.LogError($"Ammo {ammo.itemName} does not have an icon assigned.");
                }
            }
            else
            {
                Debug.LogError($"Slot {slotIndex} does not have an Image component.");
            }
        }
    }




    // 장전 완료 처리
    public void Reload()
    {
        InventoryUIManager.Instance.ToggleInventory();
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            // 턴제 모드일 경우, 플레이어의 턴에만 작동
            if (TurnManager.Instance.CurrentTurnTaker == playerCombat.GetComponent<PlayerTurnManager>())
            {
                playerCombat.LogLoadedAmmo();
                playerCombat.SetLoadedAmmo(loadedAmmo); // PlayerCombat에 장전된 탄약 전달
                playerCombat.EndTurn(); // 턴 종료
            }
        }
        else
        {
            // 턴제가 아닐 경우 언제든지 작동
            playerCombat.SetLoadedAmmo(loadedAmmo); // PlayerCombat에 장전된 탄약 전달
            AmmoManager.Instance.LogAmmoInventory();
            playerCombat.LogLoadedAmmo();
            LogChamberSlots();
            Debug.Log(loadedAmmo);
        }
        
    }


    // 장전 초기화
    public void ResetChamber()
    {
        loadedAmmo.Clear();
        foreach (var slot in chamberSlots)
        {
            slot.GetComponent<Image>().sprite = null; // 슬롯 이미지 초기화
        }
    }

}