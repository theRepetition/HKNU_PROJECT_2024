using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject player; // 플레이어 오브젝트
    public GameObject[] boundaryObjects; // 경계 오브젝트들을 배열 -> 맵에 시작위치 트리거
    public PlayerMovement playermovement;
    public Vector2 randomPositionRange; // 오브젝트 배치 시 사용할 위치 범위
    public CameraController cameraController; // 카메라 관리
    public static int currentStage = 1; // 전역 스테이지 관리 변수
    public CycleManager cycleManager; // CycleManager 참조
    private bool isStageTriggered = false; // 스테이지가 이미 발동되었는지 체크
    public static GameObject selectedBoundary; // 선택된 경계를 저장할 변수

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트가 "RightBoundary" 태그를 가지고 있고, NPC가 없을 때만 실행
        if (collision.gameObject.CompareTag("RightBoundary") && !AreNPCsPresent())
        {
            playermovement.DisableMovement(); // 플레이어 이동 멈춤
            TeleportPlayerToRandomBoundary(); // 랜덤 경계로 순간이동
            AdvanceStage(); // 스테이지 증가
        }
    }

    // 스테이지를 증가시키는 함수
    void AdvanceStage()
    {
        currentStage++; // 스테이지 증가
        Debug.Log("Current Stage: " + currentStage); // 콘솔에 현재 스테이지 출력
        isStageTriggered = false; // 다음 스테이지를 위해 트리거 리셋
    }

    // 플레이어를 랜덤 경계로 순간이동하는 함수
    void TeleportPlayerToRandomBoundary()
    {
        // 경계 목록에서 랜덤하게 하나를 선택
        int randomIndex = Random.Range(0, boundaryObjects.Length);
        GameObject selectedBoundary = boundaryObjects[randomIndex];
        
        // 플레이어를 선택된 경계로 순간이동
        player.transform.position = new Vector3(selectedBoundary.transform.position.x + 1f, selectedBoundary.transform.position.y, -2);
        Debug.Log($"Player teleported to {selectedBoundary.name}");
        cameraController.TriggerRoomTransition();

        // CycleManager에서 선택된 경계 업데이트
        cycleManager.SetRandomBoundary(selectedBoundary); // CycleManager에 경계 전달

    }
    
    // 스테이지 다른 곳에서 사용
    public int CurrentStage
    {   
        get { return currentStage; }
        
    }
    private bool AreNPCsPresent()
    {
        // 씬에 있는 NPC 오브젝트들을 찾음 ("NPC"는 실제 사용 중인 태그로 변경해야 함)
        return GameObject.FindGameObjectsWithTag("NPC").Length > 0;
    }
    public void StageReset()
    {
       currentStage = 1;
    }
}
