using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    public GameObject inventoryPanel; // 인벤토리 패널
    public GameObject ammoInventoryPanel; // 탄약 인벤토리 패널
    public GameObject ammoButtonPrefab; // 탄약 버튼 프리팹

    private InventorySlot[] slots; // 인벤토리 슬롯 배열
    private Inventory inventory; // 인벤토리 인스턴스
    public bool isActive = false;
    private List<AmmoButton> ammoButtons = new List<AmmoButton>(); // 생성된 탄약 버튼 리스트

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복된 인스턴스가 있으면 파괴
        }
        else
        {
            Instance = this; // 싱글톤 인스턴스 할당
        }
    }

    void Start()
    {
        slots = inventoryPanel.GetComponentsInChildren<InventorySlot>(); // 인벤토리 패널에 있는 슬롯을 가져옴
        inventory = Inventory.instance; // 인벤토리 인스턴스 가져옴
        inventory.onItemChangedCallback += UpdateUI; // 인벤토리 변경 시 UpdateUI 호출

        inventoryPanel.SetActive(false); // 게임 시작 시 인벤토리 패널 비활성화
        ammoInventoryPanel.SetActive(false); // 게임 시작 시 탄약 인벤토리 패널 비활성화
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.R)) && !GameStateManager.Instance.IsPauseMenuOpen()) // I 또는 R 키 입력 시
        {
            ToggleInventory(); // 인벤토리 토글
        }
    }

    public void ToggleInventory()
    {
        bool isOpen = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isOpen);
        ammoInventoryPanel.SetActive(isOpen);

        GameStateManager.Instance.SetInventoryOpen(isOpen);

        if (isOpen)
        {
            UpdateAmmoButtons();
        }
        else
        {
            // 턴제 모드가 아니거나, 플레이어 턴일 때만 이동과 전투 활성화
            if (GameModeManager.Instance.currentMode != GameModeManager.GameMode.TurnBased
                || TurnManager.Instance.CurrentTurnTaker is PlayerTurnManager)
            {
                GameStateManager.Instance.ResumeGame();
            }
        }
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        ammoInventoryPanel.SetActive(false);

        // 턴제 모드이면서 플레이어 턴이거나, 아예 턴제 모드가 아닐 때만 GameStateManager에 InventoryOpen 상태 업데이트
        if ((GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && TurnManager.Instance.CurrentTurnTaker is PlayerTurnManager)
            || GameModeManager.Instance.currentMode != GameModeManager.GameMode.TurnBased)
        {
            GameStateManager.Instance.SetInventoryOpen(false);
        }
    }


    private IEnumerator ResumeGameWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 0.5초 대기
        GameStateManager.Instance.ResumeGame(); // 게임 재개
    }

    // 인벤토리 UI를 업데이트하는 함수
    public void UpdateUI()
    {
        List<Item> items = inventory.GetItems(); // 인벤토리에서 아이템 목록 가져옴
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count && items[i] != null) // 슬롯에 아이템이 있으면
            {
                slots[i].AddItem(items[i]); // 아이템 추가
            }
            else
            {
                slots[i].ClearSlot(); // 슬롯 비우기
            }
        }
    }

    // 탄약 버튼을 업데이트하는 함수
    public void UpdateAmmoButtons()
    {
        if (AmmoManager.Instance == null)
        {
            Debug.LogError("AmmoManager instance is null!");
            return;
        }

        // 기존의 모든 탄약 버튼 제거
        foreach (Transform child in ammoInventoryPanel.transform)
        {
            Destroy(child.gameObject); // 자식 객체(버튼) 삭제
        }
        ammoButtons.Clear(); // 탄약 버튼 리스트 초기화

        List<Ammo> ammoTypes = AmmoManager.Instance.GetAmmoList(); // AmmoManager에서 탄약 목록 가져오기
        foreach (Ammo ammo in ammoTypes)
        {
            Debug.Log($"Creating button for: {ammo.itemName}, Quantity: {ammo.quantity}");
            GameObject ammoButtonObject = Instantiate(ammoButtonPrefab, ammoInventoryPanel.transform); // 탄약 버튼 생성
            AmmoButton ammoButton = ammoButtonObject.GetComponent<AmmoButton>(); // 생성된 버튼에서 AmmoButton 컴포넌트 가져오기
            ammoButton.SetAmmo(ammo); // 탄약 정보 설정
            ammoButtons.Add(ammoButton); // 리스트에 추가
        }
    }
}
