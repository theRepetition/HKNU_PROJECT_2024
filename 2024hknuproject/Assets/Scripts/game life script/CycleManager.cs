using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleManager : MonoBehaviour
{
    public List<GameObject> npcPrefabs; // 스폰할 NPC 프리팹 리스트
    public int npcCount = 5; // 생성할 NPC의 수
    public float spawnRadius = 1f; // 충돌 검사를 위한 범위(반지름)
    public LayerMask spawnLayerMask; // 충돌 검사를 위한 레이어 마스크
    public NPCTriggerManager NTM; // 보상 검사를 위한 트리거 매니저
    public StageManager stageManager;
    public static int currentStage;
    public GameObject randomBoundary;

    public void SetRandomBoundary(GameObject boundary)
    {
        randomBoundary = boundary;
        Debug.Log($"CycleManager에서 선택된 경계: {randomBoundary.name}");
    }
    private Transform GetClosestSpawnPointByLayer()
    {
        
        
        if (randomBoundary == null)
        {
            Debug.LogError("랜덤으로 선택된 경계가 설정되지 않았습니다.");
            return null;
        }

        // 레이어에 맞는 모든 오브젝트를 찾음
        Collider2D[] potentialSpawnPoints = Physics2D.OverlapCircleAll(randomBoundary.transform.position, 100f, spawnLayerMask);
        if (potentialSpawnPoints.Length == 0)
        {
            Debug.LogError("설정된 레이어에 해당하는 스폰 포인트가 없습니다.");
            return null;
        }

        // 가장 가까운 스폰 포인트를 찾음
        Transform closestPoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D spawnPoint in potentialSpawnPoints)
        {
            float distance = Vector2.Distance(randomBoundary.transform.position, spawnPoint.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = spawnPoint.transform;
            }
        }

        Debug.Log($"가장 가까운 스폰 포인트는: {closestPoint?.name} 입니다.");
        return closestPoint;
    }


    // NPC를 랜덤한 위치에 생성하는 함수
    public void SpawnNPCs()
    {
        Debug.Log("NPC 스폰 시작");

        // 가장 가까운 스폰 포인트를 찾습니다.
        Transform spawnArea = GetClosestSpawnPointByLayer();
        Debug.Log("현재 스폰포인트: " + spawnArea);
        if (spawnArea == null)
        {
            Debug.LogError("스폰 포인트를 찾을 수 없습니다.");
            return;
        }

        Bounds spawnBounds = spawnArea.GetComponent<Collider2D>().bounds;

        for (int i = 0; i < npcCount; i++)
        {
            Vector2 spawnPosition;

            // 충돌이 없는 위치를 찾을 때까지 반복
            int attempts = 0;
            do
            {
                // 스폰 포인트 영역 내에서 랜덤한 위치를 생성
                spawnPosition = new Vector2(
                    Random.Range(spawnBounds.min.x, spawnBounds.max.x),
                    Random.Range(spawnBounds.min.y, spawnBounds.max.y)
                );
                attempts++;
            }
           
            // 스폰 위치에 충돌이 없는지 확인
            while (Physics2D.OverlapCircle(spawnPosition, spawnRadius, spawnLayerMask) != null && attempts < 10);

            if (attempts >= 100)
            {
                Debug.Log(spawnArea);
                Debug.LogWarning("NPC 스폰에 실패했습니다. 스폰 가능한 공간을 찾지 못했습니다.");
                continue;
            }

            // NPC 프리팹 중 하나를 랜덤으로 선택하여 스폰 위치에 인스턴스화
            GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Count)];
            Instantiate(npcPrefab, spawnPosition, Quaternion.identity);
        }

        NTM.enableReward(); // 스폰 후 보상 처리
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // 가장 가까운 스폰 포인트를 시각적으로 표시
        Transform spawnArea = GetClosestSpawnPointByLayer();
        if (spawnArea != null)
        {
            Bounds bounds = spawnArea.GetComponent<Collider2D>().bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }


    public int npcDamageIncreasePerStage = 5; // 스테이지당 공격력 증가량
    public int npcHealthIncreasePerStage = 20;

    // 스테이지 증가 시 호출되는 함수
    public void IncreaseNPCStats()
    {   currentStage = stageManager.CurrentStage-1; // 현제 스테이지 -1 만큼(1스테이지일때 0이고 2스테이지일때 1틱 만큼 증가해야 하므로)
        // 씬 내 모든 NPC의 `NPCCombatAI` 컴포넌트를 가져옵니다.
        NPCCombatAI[] allNPCs = FindObjectsOfType<NPCCombatAI>();
        Debug.Log($"찾은 NPC 수: {allNPCs.Length}");
        foreach (var npc in allNPCs)
        {
            // 공격력 증가
            Debug.Log("스테이지 공격력:" + currentStage);
            npc.projectileDamage += (npcDamageIncreasePerStage*currentStage);
            Debug.Log($"NPC의 공격력이 증가했습니다: {npc.projectileDamage}");

            // 체력 증가 (기본 체력과 최대 체력 증가)
            NPCHealth npcHealth = npc.GetComponent<NPCHealth>();
            if (npcHealth != null)
            {
                npcHealth.currentHealth += npcHealthIncreasePerStage * currentStage; // 현재 체력 증가
                npcHealth.maxHealth += npcHealthIncreasePerStage * currentStage; // 최대 체력 증가
                Debug.Log($"NPC의 체력이 증가했습니다: 현재 체력 = {npcHealth.currentHealth}, 최대 체력 = {npcHealth.maxHealth}");
            }
        }
    }
}
