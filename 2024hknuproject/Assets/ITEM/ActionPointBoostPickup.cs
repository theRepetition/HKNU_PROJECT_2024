using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointBoostPickup : MonoBehaviour
{
    public ActionPointBoost actionPointBoostItem;

    public void PickUp()
    {
        PlayerCombat playerCombat = GameObject.FindObjectOfType<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.IncreaseMaxActionPoints(actionPointBoostItem.maxActionPointIncrease); // 최대 행동력 증가
            Debug.Log($"{actionPointBoostItem.itemName}을 사용하여 최대 행동력 {actionPointBoostItem.maxActionPointIncrease} 증가");
        }
        Destroy(gameObject); // 아이템 오브젝트 제거
    }
}
