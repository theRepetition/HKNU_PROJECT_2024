using UnityEngine;

public class NPCTriggerManager : MonoBehaviour
{
    public Collider2D leftTrigger;  // 왼쪽 경계 트리거
    public Collider2D rightTrigger; // 오른쪽 경계 트리거

    void Start()
    {
        CheckNPCCount(); // 시작 시 한 번 NPC를 체크하여 트리거 상태 결정
    }

    void Update()
    {
        CheckNPCCount(); // 매 프레임마다 NPC 수를 확인하여 트리거 상태 갱신
    }

    // NPC 태그가 달린 오브젝트의 수를 확인하는 함수
    void CheckNPCCount()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC"); // "NPC" 태그가 붙은 오브젝트를 배열로 가져옴
        int npcCount = npcs.Length; // 배열의 길이로 NPC의 수를 확인

        if (npcCount > 0)
        {
            DisableTriggers(); // NPC가 있을 경우 트리거 비활성화
        }
        else
        {
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
}
