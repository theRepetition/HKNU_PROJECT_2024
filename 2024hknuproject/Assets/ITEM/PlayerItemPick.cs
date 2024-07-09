using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemPick : MonoBehaviour
{
    public float pickupRange = 2f; // �������� �ֿ� �� �ִ� ����

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("fŰ �Է�");
            PickUpItem();
        }
    }

    void PickUpItem()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRange); // �÷��̾� �ֺ��� �ݶ��̴� �˻�

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
