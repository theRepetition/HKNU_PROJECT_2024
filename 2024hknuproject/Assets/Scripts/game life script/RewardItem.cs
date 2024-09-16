using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
    private string rewardName;
    private NPCTriggerManager manager;
    public Item item; // 줍기 가능한 아이템 정보 (ItemPickup에서 사용됨)

    // 보상 아이템의 정보를 설정하는 함수
    public void SetReward(string rewardName, NPCTriggerManager manager, Item item)
    {
        this.rewardName = rewardName;
        this.manager = manager;
        this.item = item;
        Debug.Log("Reward Set: " + rewardName);
    }

    // 플레이어가 보상과 충돌했을 때 나머지 보상 제거 및 아이템 줍기
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player selected: " + rewardName);

            // ItemPickup 스크립트를 통해 아이템 줍기
            ItemPickup itemPickup = GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.PickUp(); // 아이템 줍기
            }

            // 나머지 보상을 제거
            manager.RemoveOtherRewards(gameObject);
        }
    }
}
