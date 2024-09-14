using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTrigger : MonoBehaviour
{
    public int currentStage = 1; // 현재 스테이지를 직접 관리
    private bool isTriggered = false; // 트리거가 이미 발동했는지 체크

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered) // 플레이어가 트리거에 들어오고, 이미 발동되지 않은 경우
        {
            isTriggered = true; // 트리거는 한 번만 발동
            AdvanceStage(); // 스테이지 증가
        }
    }

    void AdvanceStage()
    {
        currentStage++; // 스테이지 증가
        Debug.Log("Current Stage: " + currentStage); // 콘솔에 현재 스테이지 출력
    }
}
