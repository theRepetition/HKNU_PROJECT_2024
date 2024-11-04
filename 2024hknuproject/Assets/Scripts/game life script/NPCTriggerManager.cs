using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerManager : MonoBehaviour
{
    public Collider2D rightTrigger;
    public GameObject rewardPrefab;
    public Transform rewardDropLocation;
    public Sprite explosiveAmmoIcon;
    public Sprite healthPotionIcon;
    public Sprite defensePackIcon;
    private bool rewardsDropped = false;
    private GameObject[] spawnedRewards = new GameObject[3];
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat; // 플레이어의 전투 제어
    // 하드코딩된 보상 목록
    private Item[] rewardPool;

    // RewardManager 참조 (UI 관리)
    public RewardManager rewardManager;

    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>(); // PlayerCombat 컴포넌트 찾기
        playerMovement = FindObjectOfType<PlayerMovement>(); // PlayerMovement 컴포넌트 찾기
                                                             // 하드코딩된 보상 목록 설정
        rewardPool = new Item[]
   {
        new Ammo("Explosive Ammo", 50, "Explodes on impact", explosiveAmmoIcon, 5),
        new HealthRecovery("Health Potion", 15, healthPotionIcon),
        new HealthBoost("Health Boost", 20, healthPotionIcon),
        new ActionPointBoost("Action Point Boost", 2, defensePackIcon)
   };
    }

    void Update()
    {
        CheckNPCCount(); // 매 프레임마다 NPC 수를 확인하여 트리거 상태 갱신
    }

    void CheckNPCCount()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        int npcCount = npcs.Length;

        if (npcCount > 0)
        {
            DisableTriggers();
        }
        else
        {
            if (!rewardsDropped)
            {
                PauseGameAndShowRewards();
                disableReward();
            }
            EnableTriggers();
        }
    }

    void DisableTriggers()
    {
        rightTrigger.enabled = false;
        Debug.Log("Triggers disabled, NPCs are present.");
    }

    void EnableTriggers()
    {
        rightTrigger.enabled = true;
        Debug.Log("Triggers enabled, all NPCs are gone.");
    }

    public void enableReward()
    {
        rewardsDropped = false;
        Debug.Log("보상 활성화");
    }

    public void disableReward()
    {
        rewardsDropped = true;
        Debug.Log("보상 비활성화");
    }

    void PauseGameAndShowRewards()
    {
        if (playerMovement != null) playerMovement.DisableMovement(); // 플레이어 이동 비활성화
        if (playerCombat != null) playerCombat.DisableCombat(); // 플레이어 전투 비활성화

        // 보상 드롭 및 UI 표시
        DropRewards();
    }

    public void ResumeGameAfterRewardSelection()
    {
        if (playerMovement != null) playerMovement.EnableMovement(); // 플레이어 이동 활성화
        if (playerCombat != null) playerCombat.EnableCombat(); // 플레이어 전투 활성화
        Debug.Log("Game resumed after reward selection.");
    }

    // 보상을 드롭하고 UI와 연동하는 함수
    void DropRewards()
    {
        Debug.Log("Displaying rewards in UI...");

        List<int> usedIndexes = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, rewardPool.Length);
            }
            while (usedIndexes.Contains(randomIndex)); // 중복 방지

            usedIndexes.Add(randomIndex);

            GameObject reward = Instantiate(rewardPrefab, rewardDropLocation.position + new Vector3(i * 2, 0, 0), Quaternion.identity);
            RewardItem rewardItem = reward.GetComponent<RewardItem>();

            if (rewardItem != null)
            {
                rewardItem.SetReward(rewardPool[randomIndex].itemName, this, rewardPool[randomIndex]);
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

        // 선택된 보상을 UI에 표시하고 버튼을 생성하여 연결
        if (rewardManager != null)
        {
            rewardManager.DisplayRewardsUI(spawnedRewards, this);
        }
    }


    // 선택되지 않은 보상을 제거하는 함수
    public void RemoveOtherRewards(GameObject selectedReward)
    {
        foreach (GameObject reward in spawnedRewards)
        {
            if (reward != selectedReward)
            {
                Destroy(reward);
            }
        }

        // 보상 선택 후 게임 재개
        ResumeGameAfterRewardSelection();
    }
}
