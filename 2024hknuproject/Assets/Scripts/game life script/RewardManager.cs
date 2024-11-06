using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public GameObject rewardUIPanel;
    public GameObject rewardButtonPrefab;
    public Transform rewardButtonContainer;
    private NPCTriggerManager npcTriggerManager;

    public void DisplayRewardsUI(GameObject[] rewards, NPCTriggerManager manager)
    {
        npcTriggerManager = manager;
        rewardUIPanel.SetActive(true);

        foreach (Transform child in rewardButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var rewardObject in rewards)
        {
            GameObject rewardButton = Instantiate(rewardButtonPrefab, rewardButtonContainer);
            RewardButton buttonComponent = rewardButton.GetComponent<RewardButton>();

            if (buttonComponent != null)
            {
                buttonComponent.Setup(rewardObject, this);
            }
        }
    }

    public void SelectReward(GameObject selectedReward)
    {
        // 플레이어 위치에 선택된 보상 아이템 스폰
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        selectedReward.transform.position = playerTransform.position;

        

        rewardUIPanel.SetActive(false);
        npcTriggerManager.RemoveOtherRewards(selectedReward);
    }
}