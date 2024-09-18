using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleManager : MonoBehaviour
{
    public List<GameObject> npcPrefabs; // 스폰할 NPC 프리팹 리스트
    public int npcCount = 5; // 생성할 NPC의 수
    public Vector2 spawnAreaMin; // 스폰 범위의 최소 좌표
    public Vector2 spawnAreaMax; // 스폰 범위의 최대 좌표
    public NPCTriggerManager NTM; //보상 검사를 위함
    public float spawnRadius = 1f; // 충돌 검사를 위한 범위(반지름)

    void Start()
    {
        SpawnNPCs(); // 스테이지 시작 시 NPC를 랜덤 위치에 스폰
        
    }

    // NPC를 랜덤한 위치에 생성하는 함수
    public void SpawnNPCs()
    {
        Debug.Log("npc 스폰");
        for (int i = 0; i < npcCount; i++)
        {
            Vector2 spawnPosition;

            // 충돌이 없는 위치를 찾을 때까지 반복
            int attempts = 0;
            do
            {
                // 랜덤한 위치 생성
                spawnPosition = new Vector2(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    Random.Range(spawnAreaMin.y, spawnAreaMax.y)
                );
                attempts++;
            }
            // 위치에 충돌이 없을 때만 스폰, 충돌이 있으면 다시 위치 찾기 (최대 10번 시도)
            while (Physics2D.OverlapCircle(spawnPosition, spawnRadius) != null && attempts < 10);

            if (attempts >= 10)
            {
                Debug.LogWarning("NPC 스폰에 실패했습니다. 스폰 가능한 공간을 찾지 못했습니다.");
                continue;
            }

            // NPC 프리팹 중 하나를 랜덤으로 선택
            GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Count)];

            // 선택된 NPC 프리팹을 스폰 위치에 인스턴스화
            Instantiate(npcPrefab, spawnPosition, Quaternion.identity);
        }
        NTM.enableReward(); // 스폰 뒤에 npc가 사라지면 보상 받을수 있게끔 초기화
    }

    // 스폰 범위 시각적으로 확인하기 위한 Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((spawnAreaMin + spawnAreaMax) / 2, spawnAreaMax - spawnAreaMin);
    }
    public int npcDamageIncreasePerStage = 5; // 스테이지당 공격력 증가량

    // 스테이지 증가 시 호출되는 함수
    public void IncreaseNPCStats()
    {
        // 씬 내 모든 NPC의 `NPCCombatAI` 컴포넌트를 가져옵니다.
        NPCCombatAI[] allNPCs = FindObjectsOfType<NPCCombatAI>();

        foreach (var npc in allNPCs)
        {
            // 현재 스테이지에 따라 공격력을 증가시킵니다.
            npc.projectileDamage += npcDamageIncreasePerStage;
            Debug.Log($"NPC의 공격력이 증가했습니다: {npc.projectileDamage}");
        }
    }
}
