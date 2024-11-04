using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemPick : MonoBehaviour
{
    public float pickupRange = 2f; // 아이템 줍기 범위
    public NPCTriggerManager npcTriggerManager; // NPCTriggerManager 참조 (RemoveOtherRewards 호출)

    void Update()
    {
        AutoPickUpItems();
    }

    void AutoPickUpItems()
    {
        // "Item" 태그가 있는 아이템 탐색
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in items)
        {
            if (Vector2.Distance(transform.position, item.transform.position) <= pickupRange)
            {
                // 아이템이 각각의 픽업 스크립트를 가지고 있는지 확인하고, 있으면 PickUp 호출
                var ammoPickup = item.GetComponent<AmmoPickup>();
                if (ammoPickup != null)
                {
                    ammoPickup.PickUp();
                    npcTriggerManager.RemoveOtherRewards(item);
                    return; // 하나 줍고 종료
                }

                var healthRecoveryPickup = item.GetComponent<HealthRecoveryPickup>();
                if (healthRecoveryPickup != null)
                {
                    healthRecoveryPickup.PickUp();
                    npcTriggerManager.RemoveOtherRewards(item);
                    return; // 하나 줍고 종료
                }

                var healthBoostPickup = item.GetComponent<HealthBoostPickup>();
                if (healthBoostPickup != null)
                {
                    healthBoostPickup.PickUp();
                    npcTriggerManager.RemoveOtherRewards(item);
                    return; // 하나 줍고 종료
                }

                var actionPointBoostPickup = item.GetComponent<ActionPointBoostPickup>();
                if (actionPointBoostPickup != null)
                {
                    actionPointBoostPickup.PickUp();
                    npcTriggerManager.RemoveOtherRewards(item);
                    return; // 하나 줍고 종료
                }
            }
        }
    

        // "Bullet" 태그가 있는 탄약 탐색
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (var bullet in bullets)
        {
            if (Vector2.Distance(transform.position, bullet.transform.position) <= pickupRange)
            {
                AmmoPickup ammoPickup = bullet.GetComponent<AmmoPickup>();
                if (ammoPickup != null)
                {
                    ammoPickup.PickUp(); // 탄약 줍기 로직 실행
                    npcTriggerManager.RemoveOtherRewards(bullet); // 선택되지 않은 나머지 보상 제거
                    return; // 하나 줍고 종료
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
