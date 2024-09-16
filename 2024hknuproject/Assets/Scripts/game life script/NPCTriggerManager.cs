using UnityEngine;

public class NPCTriggerManager : MonoBehaviour
{
    public Collider2D leftTrigger;  // 왼쪽 경계 트리거
    public Collider2D rightTrigger; // 오른쪽 경계 트리거
    public GameObject rewardPrefab; // 보상 아이템의 프리팹 (3개의 아이템으로 복제)
    public Transform rewardDropLocation; // 보상이 드랍될 위치

    private bool rewardsDropped = false; // 보상이 드랍되었는지 확인
    private GameObject[] spawnedRewards = new GameObject[3]; // 드랍된 보상들을 저장하는 배열

    // 하드코딩된 보상 풀 (간단한 예시)
    private Item[] rewardPool;

    void Start()
    {
        // 하드코딩된 보상 풀 설정
        rewardPool = new Item[]
        {
            new Ammo("Explosive Ammo", 50, "Explodes on impact", null, 5), // Ammo 아이템
            new Consumable { itemName = "Health Potion", healthRestore = 50 }, // Consumable 아이템
            new Weapon { itemName = "Sword", damage = 25, range = 1.5f } // Weapon 아이템
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

        // 보상 풀에서 3개의 랜덤 아이템 선택 및 생성
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, rewardPool.Length);

            // 보상 아이템 생성 및 위치 설정
            GameObject reward = Instantiate(rewardPrefab, rewardDropLocation.position + new Vector3(i * 2, 0, 0), Quaternion.identity);
            reward.GetComponent<RewardItem>().SetReward(rewardPool[randomIndex].itemName, this, rewardPool[randomIndex]); // 아이템 설정 및 관리
            spawnedRewards[i] = reward;
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
