using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardManager : MonoBehaviour
{
    public GameObject rewardUIPanel;
    public GameObject rewardButtonPrefab;
    public Transform rewardButtonContainer;
    public TextMeshProUGUI itemNameText; // 오브젝트 이름을 표시할 TMP UI 텍스트

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

    // 오브젝트 이름을 표시하는 메서드, 특정 버튼 위에 띄우기
    public void ShowItemName(GameObject rewardObject, RectTransform buttonTransform)
    {
        if (itemNameText != null)
        {
            // "(Clone)" 텍스트 제거
            string itemName = rewardObject.name.Replace("(Clone)", "").Trim();
            itemNameText.text = itemName;
            itemNameText.gameObject.SetActive(true);

            // 버튼 위치를 기준으로 약간 위에 표시
            Vector3 buttonPosition = buttonTransform.position;
            itemNameText.transform.position = buttonPosition + new Vector3(0, buttonTransform.rect.height * 0.6f, 0);
        }
    }

    // 오브젝트 이름을 숨기는 메서드
    public void HideItemName()
    {
        if (itemNameText != null)
        {
            itemNameText.gameObject.SetActive(false); // 텍스트 숨기기
        }
    }

    public void SelectReward(GameObject selectedReward)
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        selectedReward.transform.position = playerTransform.position;

        rewardUIPanel.SetActive(false);
        GameStateManager.Instance.SetRewardUIOpen(false);

        npcTriggerManager.RemoveOtherRewards(selectedReward);
    }
}
