using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject player; // 플레이어 오브젝트
    public GameObject leftBoundaryObject; // 왼쪽 경계 오브젝트
    public GameObject rightBoundaryObject; // 오른쪽 경계 오브젝트
    public GameObject[] objectsToRandomize; // 셀 내 랜덤 배치될 오브젝트들
    public Vector2 randomPositionRange; // 오브젝트 배치 시 사용할 위치 범위
    public CameraController cameraController;
    public int currentStage = 1; // 현재 스테이지 관리
    private bool isStageTriggered = false; // 스테이지가 이미 발동되었는지 체크

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 스테이지 트리거: 플레이어가 스테이지 트리거에 닿으면
        if (collision.CompareTag("Player") && !isStageTriggered) 
        {
            Debug.Log("Player object: " + player.name);
            Debug.Log(rightBoundaryObject.transform.position.x);
            Debug.Log(leftBoundaryObject.transform.position.x);
            Debug.Log(collision.gameObject);
            isStageTriggered = true; // 트리거 한 번만 발동
            AdvanceStage(); // 스테이지 증가
        }

        // 왼쪽 경계에 닿았을 때 오른쪽 경계로 이동
        if (collision.gameObject == leftBoundaryObject)
        {
            TeleportPlayerToRight();
            RandomizeObjects();
        }
        // 오른쪽 경계에 닿았을 때 왼쪽 경계로 이동
        else if (collision.gameObject == rightBoundaryObject)
        {
            TeleportPlayerToLeft();
            RandomizeObjects();
        }
    }

    // 스테이지를 증가시키는 함수
    void AdvanceStage()
    {   
        currentStage++; // 스테이지 증가
        Debug.Log("Current Stage: " + currentStage); // 콘솔에 현재 스테이지 출력
        isStageTriggered = false; // 다음 스테이지를 위해 트리거 리셋
        Debug.Log("Current Stage: " + currentStage); // 콘솔에 현재 스테이지 출력
    }

    // 플레이어를 오른쪽 경계로 순간이동
    void TeleportPlayerToRight()
    {
        player.transform.position = new Vector2(rightBoundaryObject.transform.position.x - 1f, player.transform.position.y); // 오른쪽 경계로 순간이동
        Debug.Log("Player teleported to the right.");
        cameraController.TriggerRoomTransition();
    }

    // 플레이어를 왼쪽 경계로 순간이동
    void TeleportPlayerToLeft()
    {
        player.transform.position = new Vector2(leftBoundaryObject.transform.position.x + 1f, player.transform.position.y); // 왼쪽 경계로 순간이동
        Debug.Log("Player teleported to the left.");
        cameraController.TriggerRoomTransition();
    }

    // 오브젝트들을 랜덤하게 배치하는 함수
    void RandomizeObjects()
    {
        foreach (GameObject obj in objectsToRandomize)
        {
            float randomX = Random.Range(-randomPositionRange.x, randomPositionRange.x);
            float randomY = Random.Range(-randomPositionRange.y, randomPositionRange.y);
            obj.transform.position = new Vector2(randomX, randomY);
        }

        Debug.Log("Objects randomized.");
    }
}
