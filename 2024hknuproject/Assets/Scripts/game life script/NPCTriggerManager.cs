using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerManager : MonoBehaviour
{
    public Collider2D leftTrigger;  // 왼쪽 경계 트리거
    public Collider2D rightTrigger; // 오른쪽 경계 트리거
    public GameObject rewardPrefab; // 보상 아이템의 프리팹 (SpriteRenderer가 포함된 오브젝트)
    public Transform rewardDropLocation; // 보상 아이템이 드랍될 위치의 기준점
    public Sprite explosiveAmmoIcon;
    public Sprite healthPotionIcon;
    public Sprite defensePackIcon;
    private bool rewardsDropped = false; // 보상이 드랍되었는지 확인
    private GameObject[] spawnedRewards = new GameObject[3]; // 드랍된 보상들을 저장하는 배열

    // 하드코딩된 보상 목록
    private Item[] rewardPool;

    void Start()
    {
        // 하드코딩된 보상 목록 설정
        rewardPool = new Item[]
        {
            new Ammo("Explosive Ammo", 50, "Explodes on impact", explosiveAmmoIcon, 5), // 폭발 탄환
            new Consumable { itemName = "Health Potion", healthRestore = 15, icon = healthPotionIcon }, // 체력 물약
            new Item { itemName = "Defense Pack", description = "Reduces incoming damage by 3", icon = defensePackIcon } // 방탄팩
        };

        CheckNPCCount(); // 시작 시 한 번 NPC를 체크하여 트리거 상태 결정
    }

    void Update()
    {
        CheckNPCCount(); // 매 프레임마다 NPC 수를 확인하여 트리거 상태 갱신
    }

    // NPC 태그가 달린 오브젝트의 수를 확인하는 함수
    void CheckNPCCount()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        int npcCount = npcs.Length;

        if (npcCount > 0)
        {
            DisableTriggers(); // NPC가 있을 경우 트리거 비활성화
        }
        else
        {
            if (!rewardsDropped)
            {
                DropRewards(); // NPC가 없을 경우 보상 드랍
                rewardsDropped = true; // 보상은 한 번만 드랍
            }
            EnableTriggers(); // NPC가 없을 경우 트리거 활성화
        }
    }

    // 트리거를 비활성화하는 함수
    void DisableTriggers()
    {
        leftTrigger.enabled = false;
        rightTrigger.enabled = false;
        Debug.Log("Triggers disabled, NPCs are present.");
    }

    // 트리거를 활성화하는 함수
    void EnableTriggers()
    {
        leftTrigger.enabled = true;
        rightTrigger.enabled = true;
        Debug.Log("Triggers enabled, all NPCs are gone.");
    }

    // 보상을 드랍하는 함수
    void DropRewards()
    {
        Debug.Log("Dropping rewards...");

        // 3개의 서로 다른 랜덤 아이템 선택
        List<int> usedIndexes = new List<int>(); // 이미 사용된 인덱스를 추적
        for (int i = 0; i < 3; i++)
        {
            int randomIndex;

            // 중복되지 않도록 보상 선택
            do
            {
                randomIndex = Random.Range(0, rewardPool.Length);
            }
            while (usedIndexes.Contains(randomIndex));

            usedIndexes.Add(randomIndex); // 사용한 인덱스 기록

            // 보상 오브젝트 생성 및 위치 설정
            GameObject reward = Instantiate(rewardPrefab, rewardDropLocation.position + new Vector3(i * 2, 0, 0), Quaternion.identity);

            // RewardItem 컴포넌트를 찾고, 보상 데이터를 적용
            RewardItem rewardItem = reward.GetComponent<RewardItem>();
            if (rewardItem != null)
            {
                // 보상 풀의 데이터를 프리팹에 적용
                rewardItem.SetReward(rewardPool[randomIndex].itemName, this, rewardPool[randomIndex]);

                // SpriteRenderer를 통해 아이템 아이콘을 설정
                SpriteRenderer spriteRenderer = reward.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = rewardPool[randomIndex].icon;
                }

                spawnedRewards[i] = reward;
            }
            else
            {
                Debug.LogError("RewardItem component not found on rewardPrefab!");
            }
        }
    }

    // 플레이어가 선택한 아이템 이외의 나머지 아이템을 제거하는 함수
    public void RemoveOtherRewards(GameObject selectedReward)
    {
        foreach (GameObject reward in spawnedRewards)
        {
            if (reward != selectedReward)
            {
                Destroy(reward); // 선택되지 않은 보상 제거
            }
        }
    }
}
