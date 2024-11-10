using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RewardButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 버튼 위에 있을 때 오브젝트 이름 표시
        RectTransform buttonTransform = GetComponent<RectTransform>();
        rewardManager.ShowItemName(rewardObject, buttonTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 버튼에서 벗어날 때 오브젝트 이름 숨기기
        rewardManager.HideItemName();
    }
}
