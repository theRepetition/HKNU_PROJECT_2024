using UnityEngine;

public class RewardItem : MonoBehaviour
{
    private string rewardName;
    private NPCTriggerManager manager;
    public Item item; // 줍기 가능한 아이템 정보
    private SpriteRenderer spriteRenderer; // 필드에 표시될 아이콘
    public ItemPickup itemPickup; // ItemPickup 스크립트 참조

    void Start()
    {
        // SpriteRenderer 컴포넌트 찾기
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer없음");
        }

        // ItemPickup 스크립트 찾기
        itemPickup = GetComponent<ItemPickup>();
        if (itemPickup == null)
        {
            Debug.LogError("ItemPickup없음");
        }
    }

    // 보상 아이템의 정보를 설정하는 함수
    public void SetReward(string rewardName, NPCTriggerManager manager, Item item)
    {
        if (itemPickup != null) // ItemPickup이 null이 아닌지 체크
        {
            itemPickup.item = item; // RewardItem에서 설정된 item 정보를 ItemPickup에 전달
            Debug.Log("pickup에 보냄");
        }
        else
        {
            Debug.LogError("ItemPickup 없음");
        }
        this.rewardName = rewardName;
        this.manager = manager;
        this.item = item;
        Debug.Log("Reward Set: " + rewardName);

       
        // 아이콘을 설정 (SpriteRenderer를 통해 아이템 아이콘을 표시)
        SetIcon(item.icon);
    }

    // 아이콘 설정 (SpriteRenderer에 스프라이트 할당)
    public void SetIcon(Sprite icon)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = icon; // 아이템 아이콘을 설정
        }
        else
        {
            Debug.LogError("SpriteRenderer not assigned or missing.");
        }
    }

    
}
