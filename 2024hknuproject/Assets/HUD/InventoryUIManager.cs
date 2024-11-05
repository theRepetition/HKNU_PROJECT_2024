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

    private PlayerMovement playerMovement; // 플레이어의 이동 제어
    private PlayerCombat playerCombat; // 플레이어의 전투 제어
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

        playerMovement = FindObjectOfType<PlayerMovement>(); // PlayerMovement 컴포넌트 찾기
        playerCombat = FindObjectOfType<PlayerCombat>(); // PlayerCombat 컴포넌트 찾기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.R)) // I 또는 R 키 입력 시
        {
            playerMovement.DisableMovement();
            playerCombat.DisableCombat();
            ToggleInventory(); // 인벤토리 토글
        }
    }

    public  void ToggleInventory()
    {
        // 인벤토리 활성화 상태 반전
        isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive); // 인벤토리 패널 활성화/비활성화
        ammoInventoryPanel.SetActive(isActive); // 탄약 패널 활성화/비활성화

        if (isActive) // 인벤토리가 활성화되면
        {
            UpdateAmmoButtons();
            if (playerMovement != null) playerMovement.DisableMovement(); // 플레이어 이동 비활성화
            if (playerCombat != null) playerCombat.DisableCombat(); // 플레이어 전투 비활성화

             // 탄약 버튼 업데이트
        }
        else
        {
            if (playerMovement != null) playerMovement.EnableMovement(); // 플레이어 이동 활성화
            if (playerCombat != null) playerCombat.EnableCombat(); // 플레이어 전투 활성화
        }
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false); // 인벤토리 패널 비활성화
        ammoInventoryPanel.SetActive(false); // 탄약 패널 비활성화
        StartCoroutine(EnableMovementAndCombatWithDelay(0.5f));
    }
    private IEnumerator EnableMovementAndCombatWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 0.5초 대기

        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            var playerTurnManager = playerCombat.GetComponent<PlayerTurnManager>();
            if (playerTurnManager != null && TurnManager.Instance.CurrentTurnTaker == playerTurnManager)
            {
                if (playerMovement != null) playerMovement.EnableMovement(); // 턴제에서 플레이어의 이동 활성화
                if (playerCombat != null) playerCombat.EnableCombat(); // 턴제에서 플레이어 전투 활성화
            }
        }
        else
        {
            // 실시간 모드에서는 바로 이동 및 전투 활성화
            if (playerMovement != null) playerMovement.EnableMovement();
            if (playerCombat != null) playerCombat.EnableCombat();
        }
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
