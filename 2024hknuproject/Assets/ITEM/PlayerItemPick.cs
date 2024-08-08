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
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        foreach (var item in items)
        {
            if (Vector2.Distance(transform.position, item.transform.position) <= pickupRange)
            {
                ItemPickup itemPickup = item.GetComponent<ItemPickup>();
                if (itemPickup != null)
                {
                    itemPickup.PickUp();
                    break;
                }
            }
        }

        foreach (var bullet in bullets)
        {
            if (Vector2.Distance(transform.position, bullet.transform.position) <= pickupRange)
            {
                AmmoPickup ammoPickup = bullet.GetComponent<AmmoPickup>();
                if (ammoPickup != null)
                {
                    ammoPickup.PickUp();
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
