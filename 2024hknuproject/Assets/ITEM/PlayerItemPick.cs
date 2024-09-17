using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemPick : MonoBehaviour
{
    public float pickupRange = 2f; // 아이템 줍기 범위
    public NPCTriggerManager npcTriggerManager; // NPCTriggerManager 참조 (RemoveOtherRewards 호출)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("f 누름");
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
                    itemPickup.PickUp(); // 아이템 줍기 로직 실행
                    npcTriggerManager.RemoveOtherRewards(item); // 선택되지 않은 나머지 보상을 제거
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
                    ammoPickup.PickUp(); // 탄약 줍기 로직 실행
                    npcTriggerManager.RemoveOtherRewards(bullet); // 선택되지 않은 나머지 보상을 제거
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
