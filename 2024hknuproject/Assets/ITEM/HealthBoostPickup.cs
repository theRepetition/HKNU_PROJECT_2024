using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBoostPickup : MonoBehaviour
{
    public HealthBoost healthBoostItem;

    public void PickUp()
    {
        PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.IncreaseMaxHealth(healthBoostItem.maxHealthIncrease); // 최대 체력 증가
            Debug.Log($"{healthBoostItem.itemName}을 사용하여 최대 체력 {healthBoostItem.maxHealthIncrease} 증가");
        }
        Destroy(gameObject); // 아이템 오브젝트 제거
    }
}
