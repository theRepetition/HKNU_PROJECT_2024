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
    public CycleManager cycleManager;
    public static int currentStage = 1; // 전역 스테이지 관리 변수
    private bool isStageTriggered = false; // 스테이지가 이미 발동되었는지 체크
    public GameObject rightBoundaryObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 경계에 닿았을 때
        if (collision.gameObject == rightBoundaryObject)
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
        player.transform.position = new Vector2(selectedBoundary.transform.position.x + 1f, player.transform.position.y);
        Debug.Log($"Player teleported to {selectedBoundary.name}");
        cameraController.TriggerRoomTransition();
    }
}
