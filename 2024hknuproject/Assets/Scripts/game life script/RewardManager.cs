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
        rewardUIPanel.SetActive(true);
        GameStateManager.Instance.SetRewardUIOpen(true);

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
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        selectedReward.transform.position = playerTransform.position;

        rewardUIPanel.SetActive(false);
        GameStateManager.Instance.SetRewardUIOpen(false); // 보상 UI 닫힘

        npcTriggerManager.RemoveOtherRewards(selectedReward);
    }

}
