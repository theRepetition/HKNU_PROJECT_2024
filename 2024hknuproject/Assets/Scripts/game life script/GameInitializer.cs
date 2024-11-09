using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public CycleManager cycleManager; // CycleManager 참조
  

    private void start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log("게임 초기화 시작");

        // 스테이지 초기화
        CycleManager.currentStage = 1;
        

        // NPC 스폰
        if (cycleManager != null)
        {
            cycleManager.SpawnNPCs();
        }
        else
        {
            Debug.LogWarning("CycleManager가 할당되지 않았습니다.");
        }

        Debug.Log("게임 초기화 완료");
    }
}
