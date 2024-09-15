using UnityEngine;

public class CellManager : MonoBehaviour
{
    public GameObject player;
    public GameObject leftBoundaryObject; // 왼쪽 경계 오브젝트 직접 설정
    public GameObject rightBoundaryObject; // 오른쪽 경계 오브젝트 직접 설정
    public GameObject[] objectsToRandomize; // 셀 내 랜덤 배치될 오브젝트들
    public Vector2 randomPositionRange; // 오브젝트 배치 시 사용할 위치 범위

    // 플레이어가 트리거로 왼쪽 또는 오른쪽 경계에 부딪히면 호출
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == leftBoundaryObject) // 왼쪽 경계 트리거에 부딪히면
        {
            TeleportPlayerToRight();
            RandomizeObjects();
        }
        else if (collision.gameObject == rightBoundaryObject) // 오른쪽 경계 트리거에 부딪히면
        {
            TeleportPlayerToLeft();
            RandomizeObjects();
        }
    }

    // 플레이어를 오른쪽 경계로 순간이동
    void TeleportPlayerToRight()
    {
        // 오른쪽 경계의 위치를 오브젝트의 Transform으로 받아옴
        player.transform.position = new Vector2(rightBoundaryObject.transform.position.x - 1f, player.transform.position.y); // 약간의 여유를 줌
        Debug.Log("Player teleported to the right.");
    }

    // 플레이어를 왼쪽 경계로 순간이동
    void TeleportPlayerToLeft()
    {
        // 왼쪽 경계의 위치를 오브젝트의 Transform으로 받아옴
        player.transform.position = new Vector2(leftBoundaryObject.transform.position.x + 1f, player.transform.position.y); // 약간의 여유를 줌
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
