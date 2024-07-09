using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemPick : MonoBehaviour
{
    public float pickupRange = 2f; // 아이템을 주울 수 있는 범위

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("f키 입력");
            PickUpItem();
        }
    }

    void PickUpItem()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRange); // 플레이어 주변의 콜라이더 검사

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Item"))
            {
                ItemPickup itemPickup = collider.GetComponent<ItemPickup>();
                if (itemPickup != null)
                {
                    itemPickup.PickUp();
                    break;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
