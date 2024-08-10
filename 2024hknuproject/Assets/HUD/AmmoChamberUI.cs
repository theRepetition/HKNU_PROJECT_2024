using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AmmoChamberUI : MonoBehaviour
{
    public GameObject chamberSlotPrefab; // źȯ ���� ������
    public Transform chamberPanel; // źȯ�� ��ġ�� �г�
    public Button reloadButton; // ���� �Ϸ� ��ư
    public Button resetButton; // �ʱ�ȭ ��ư

    private List<GameObject> chamberSlots = new List<GameObject>(); // ���� ������ źȯ ���Ե�
    private List<Ammo> loadedAmmo = new List<Ammo>(); // ���� ������ ź�� ���
    private PlayerCombat playerCombat;
    private InventoryUIManager inventoryUIManager;

    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        inventoryUIManager = FindObjectOfType<InventoryUIManager>();
        reloadButton.onClick.AddListener(Reload);
        resetButton.onClick.AddListener(ResetChamber);

        InitializeChamberSlots(6); // ��ǿ� ���� 6�� ���� 
    }

    // ��� ���� �ʱ�ȭ
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

    // ź�� ����
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




    // ���� �Ϸ� ó��
    public void Reload()
    {
        InventoryUIManager.Instance.ToggleInventory();
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            // ���� ����� ���, �÷��̾��� �Ͽ��� �۵�
            if (TurnManager.Instance.CurrentTurnTaker == playerCombat.GetComponent<PlayerTurnManager>())
            {
                playerCombat.LogLoadedAmmo();
                playerCombat.SetLoadedAmmo(loadedAmmo); // PlayerCombat�� ������ ź�� ����
                playerCombat.EndTurn(); // �� ����
            }
        }
        else
        {
            // ������ �ƴ� ��� �������� �۵�
            playerCombat.SetLoadedAmmo(loadedAmmo); // PlayerCombat�� ������ ź�� ����
            AmmoManager.Instance.LogAmmoInventory();
            playerCombat.LogLoadedAmmo();
            LogChamberSlots();
            Debug.Log(loadedAmmo);
        }
        
    }


    // ���� �ʱ�ȭ
    public void ResetChamber()
    {
        loadedAmmo.Clear();
        foreach (var slot in chamberSlots)
        {
            slot.GetComponent<Image>().sprite = null; // ���� �̹��� �ʱ�ȭ
        }
    }

}