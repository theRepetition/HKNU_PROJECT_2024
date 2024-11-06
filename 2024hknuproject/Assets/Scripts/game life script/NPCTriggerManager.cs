using System.Collections.Generic;
using UnityEngine;

public class NPCTriggerManager : MonoBehaviour
{
    public GameObject[] rewardPool; // 인스펙터에서 할당할 여러 보상 프리팹들
    public GameObject rewardUIPanel;
    private bool rewardsDropped = false;
    private GameObject[] spawnedRewards = new GameObject[3];

    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;

    public RewardManager rewardManager;

    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        CheckNPCCount();
    }

    void CheckNPCCount()
    {
        if (GameObject.FindGameObjectsWithTag("NPC").Length == 0 && !rewardsDropped)
        {
            PauseGameAndShowRewards();
            rewardsDropped = true;
        }
    }

    void PauseGameAndShowRewards()
{
    GameStateManager.Instance.SetRewardUIOpen(true);
    ShowRewardsUI();
}

    void ShowRewardsUI()
    {
        List<int> usedIndexes = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, rewardPool.Length);
            }
            while (usedIndexes.Contains(randomIndex));

            usedIndexes.Add(randomIndex);

            // 프리팹을 사용하여 아이템 생성
            GameObject reward = Instantiate(rewardPool[randomIndex], transform.position, Quaternion.identity);
            RewardItem rewardItem = reward.GetComponent<RewardItem>();

            if (rewardItem != null)
            {
                rewardItem.SetReward(rewardItem.item.itemName, this, rewardItem.item);
                spawnedRewards[i] = reward;
            }
            else
            {
                Debug.LogError("RewardItem component not found on rewardPrefab!");
            }
        }

        // UI에 보상 버튼을 표시
        rewardManager?.DisplayRewardsUI(spawnedRewards, this);
    }

    public void RemoveOtherRewards(GameObject selectedReward)
    {
        foreach (GameObject reward in spawnedRewards)
        {
            if (reward != selectedReward)
            {
                Destroy(reward);
            }
        }
        GameStateManager.Instance.SetRewardUIOpen(false);
    }

    public void ResumeGameAfterRewardSelection()
    {
        GameStateManager.Instance.ResumeGame();
    }

    public void enableReward()
    {
        rewardsDropped = false;
        Debug.Log("Rewards have been enabled");
    }

    public void disableReward()
    {
        rewardsDropped = true;
        Debug.Log("Rewards have been disabled");
    }
}
