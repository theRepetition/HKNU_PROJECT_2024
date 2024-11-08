using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoChamberUI : MonoBehaviour
{
    public GameObject chamberSlotPrefab; // 탄환 슬롯을 위한 프리팹
    public Transform chamberPanel; // 탄환 슬롯들이 위치할 패널
    public Button reloadButton; // 재장전 버튼
    public Button resetButton; // 초기화 버튼
    private PlayerTurnManager playerTurnManager; // 플레이어 턴 매니저 참조
    private List<GameObject> chamberSlots = new List<GameObject>(); // 생성된 탄환 슬롯들
    private List<Ammo> loadedAmmo = new List<Ammo>(); // 장전된 탄약 리스트
    private PlayerCombat playerCombat; // 플레이어 전투 관리
    private InventoryUIManager inventoryUIManager; // 인벤토리 UI 관리자
    public Sprite customimage;
    public bool ammoisfull;
    void Start()
    {
        playerTurnManager = FindObjectOfType<PlayerTurnManager>(); // PlayerTurnManager를 찾아서 할당
        playerCombat = FindObjectOfType<PlayerCombat>(); // PlayerCombat을 찾아서 할당
        inventoryUIManager = FindObjectOfType<InventoryUIManager>(); // InventoryUIManager를 찾아서 할당

        resetButton.onClick.AddListener(ResetChamber); // 초기화 버튼에 ResetChamber 함수 연결

        InitializeChamberSlots(6); // 초기 슬롯 개수 6개로 설정
    }

    // 탄환 슬롯을 초기화하는 함수
    void InitializeChamberSlots(int slotCount)
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(chamberSlotPrefab, chamberPanel); // 슬롯 프리팹을 패널에 생성
            chamberSlots.Add(slot); // 생성된 슬롯을 리스트에 추가
        }
    }

    // 현재 장전된 슬롯 정보 로깅 함수
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

    // 탄약을 장전하는 함수
    public void LoadAmmo(Ammo ammo)
    {
        if (loadedAmmo.Count < chamberSlots.Count)
        {
            loadedAmmo.Add(ammo); // 장전된 탄약 리스트에 추가
            var slotIndex = loadedAmmo.Count - 1; // 슬롯 인덱스 설정
            var slotImage = chamberSlots[slotIndex].GetComponent<Image>(); // 슬롯의 이미지 컴포넌트 가져옴

            if (slotImage != null)
            {
                if (ammo.icon != null) // 아이콘이 설정되어 있으면
                {
                    slotImage.sprite = ammo.icon; // 슬롯 이미지에 탄약 아이콘 설정
                    Debug.Log($"Slot {slotIndex} updated with {ammo.itemName} icon.");
                }
                
            }
            
        }
       
     }
    public int Ammoisfull()
    {

        return loadedAmmo.Count;
    }

    // 재장전 처리 함수
    public void Reload()
    {
        Debug.Log("잘되나2");
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            Debug.Log("잘되나1");
            if (TurnManager.Instance.CurrentTurnTaker == playerCombat.GetComponent<PlayerTurnManager>())
            {
                playerCombat.SetLoadedAmmo(loadedAmmo);
                // GameStateManager에 플레이어 행동 제한을 요청
                GameStateManager.Instance.PauseGame();
                Debug.Log("잘되나");
                playerTurnManager.EndTurn();

                

                loadedAmmo.Clear();
            }
        }
        
        else
        {
            // 실시간 모드에서 재장전 처리
            playerCombat.SetLoadedAmmo(loadedAmmo); // PlayerCombat에 장전된 탄약 리스트 전달
            
        }

        // 재장전 후 슬롯의 이미지 초기화
        foreach (var slot in chamberSlots)
        {
            slot.GetComponent<Image>().sprite = customimage; // 슬롯 이미지 비우기
        }
        InventoryUIManager.Instance.CloseInventory(); // 인벤토리 닫기
    }

    // 탄약 슬롯 초기화 함수
    public void ResetChamber()
    {
        
        foreach (var ammo in loadedAmmo)
        {   
            // 탄약 매니저에서 동일한 이름의 탄약을 찾아 인벤토리에 추가
            Ammo existingAmmo = AmmoManager.Instance.ammoTypes.Find(a => a.itemName == ammo.itemName);
            if (existingAmmo != null)
            {
                // 인벤토리에 동일한 탄약이 있을 경우 수량 증가
                existingAmmo.quantity += 1;
            }
            else
            {
                // 인벤토리에 동일한 탄약이 없을 경우 새로 추가
                AmmoManager.Instance.ammoTypes.Add(new Ammo(ammo.itemName, ammo.damage, ammo.effect, ammo.icon, 1));
            }
        }

        loadedAmmo.Clear(); // 장전된 탄약 리스트 초기화

        // 모든 슬롯 이미지 초기화
        foreach (var slot in chamberSlots)
        {
            slot.GetComponent<Image>().sprite = customimage;
        }
    }
}
