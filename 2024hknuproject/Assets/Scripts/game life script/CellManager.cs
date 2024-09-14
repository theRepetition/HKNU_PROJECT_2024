using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    public GameObject player;
    public GameObject[] objectsToRandomize; // 셀 내 랜덤 배치될 오브젝트들
    public Vector2 randomPositionRange; // 오브젝트 배치 시 사용할 위치 범위

    // 플레이어가 트리거로 왼쪽 또는 오른쪽 경계에 부딪히면 호출
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LeftBoundary")) // 왼쪽 경계 트리거에 부딪히면
        {
            TeleportPlayerToRight();
            RandomizeObjects();
        }
        else if (collision.CompareTag("RightBoundary")) // 오른쪽 경계 트리거에 부딪히면
        {
            TeleportPlayerToLeft();
            RandomizeObjects();
        }
    }

    // 플레이어를 오른쪽 경계로 순간이동
    void TeleportPlayerToRight()
    {
        // 오른쪽 경계의 위치를 트리거의 Transform으로 받아옴
        Transform rightBoundary = GameObject.FindWithTag("RightBoundary").transform;
        player.transform.position = new Vector2(rightBoundary.position.x, player.transform.position.y);
        Debug.Log("Player teleported to the right.");
    }

    // 플레이어를 왼쪽 경계로 순간이동
    void TeleportPlayerToLeft()
    {
        // 왼쪽 경계의 위치를 트리거의 Transform으로 받아옴
        Transform leftBoundary = GameObject.FindWithTag("LeftBoundary").transform;
        player.transform.position = new Vector2(leftBoundary.position.x, player.transform.position.y);
        Debug.Log("Player teleported to the left.");
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
