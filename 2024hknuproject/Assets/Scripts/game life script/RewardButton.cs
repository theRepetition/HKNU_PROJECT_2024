using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    private GameObject rewardObject;
    private RewardManager rewardManager;

    public void Setup(GameObject reward, RewardManager manager)
    {
        rewardObject = reward;
        rewardManager = manager;

        SpriteRenderer spriteRenderer = rewardObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            GetComponent<Image>().sprite = spriteRenderer.sprite;
        }
    }

    public void OnClick()
    {
        rewardManager.SelectReward(rewardObject);
    }
}
