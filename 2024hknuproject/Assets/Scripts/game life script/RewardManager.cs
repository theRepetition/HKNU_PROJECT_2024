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

        // 이전에 생성된 버튼 제거
        foreach (Transform child in rewardButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // 각 보상 오브젝트에 대한 버튼 생성 및 설정
        foreach (var rewardObject in rewards)
        {
            GameObject rewardButton = Instantiate(rewardButtonPrefab, rewardButtonContainer);
            RewardButton buttonComponent = rewardButton.GetComponent<RewardButton>();

            if (buttonComponent != null)
            {
                buttonComponent.Setup(rewardObject, this); // 보상 오브젝트 연결
            }
        }
    }

    public void SelectReward(GameObject selectedReward)
    {
        npcTriggerManager.RemoveOtherRewards(selectedReward);
        rewardUIPanel.SetActive(false);
        npcTriggerManager.ResumeGameAfterRewardSelection();
    }
}
