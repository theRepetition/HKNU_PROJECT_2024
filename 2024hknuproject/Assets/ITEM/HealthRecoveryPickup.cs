using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRecoveryPickup : MonoBehaviour
{
    public HealthRecovery healthRecoveryItem;

    public void PickUp()
    {
        PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healthRecoveryItem.healthRestore); // 체력 회복
            Debug.Log($"{healthRecoveryItem.itemName}을 사용하여 체력 {healthRecoveryItem.healthRestore} 회복");
        }
        Destroy(gameObject); // 아이템 오브젝트 제거
    }
}
