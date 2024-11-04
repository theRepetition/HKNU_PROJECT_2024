using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // 모든 아이템의 공통 데이터

    // 각 아이템에 대한 효과량을 조정할 변수들
    public int healthRestoreAmount = 0;
    public int maxHealthIncreaseAmount = 0;
    public int maxActionPointIncreaseAmount = 0;
    public int ammoQuantity = 0;

    private void Start()
    {
        // 아이템에 맞는 효과량 설정
        SetItemEffectValues();
    }

    private void SetItemEffectValues()
    {
        if (item == null)
        {
            Debug.LogError("아이템 정보가 없습니다!");
            return;
        }

        // 아이템의 타입에 따라 맞는 효과량을 설정
        switch (item.itemType)
        {
            case Item.ItemType.HealthRecovery:
                ((HealthRecovery)item).healthRestore = healthRestoreAmount;
                break;

            case Item.ItemType.HealthBoost:
                ((HealthBoost)item).maxHealthIncrease = maxHealthIncreaseAmount;
                break;

            case Item.ItemType.ActionPointBoost:
                ((ActionPointBoost)item).maxActionPointIncrease = maxActionPointIncreaseAmount;
                break;

            case Item.ItemType.Ammo:
                ((Ammo)item).quantity = ammoQuantity;
                break;

            default:
                Debug.LogWarning($"{item.itemName}은 처리되지 않은 아이템 유형입니다.");
                break;
        }
    }

    public void PickUp()
    {
        if (item == null)
        {
            Debug.LogError("아이템 정보가 없습니다!");
            return;
        }

        // 아이템의 Use() 메서드를 호출하여 효과 적용
        item.Use();
        Debug.Log("아이템 사용");
        // 아이템을 사용한 후 오브젝트 제거
        Destroy(gameObject);
    }
}
