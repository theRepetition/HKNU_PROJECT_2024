using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 5.0f; // WASD 키 입력에 의한 카메라 이동 속도
    public float mouseEdgeScrollSpeed = 3.0f; // 마우스 가장자리 이동 속도
    public float edgeSize = 10.0f; // 화면 가장자리 영역 크기
    public float maxMouseEdgeDistance = 50.0f; // 마우스 이동에 의한 최대 카메라 이동 범위
    public Transform player; // 플레이어 위치를 추적하기 위한 변수

    private Vector3 offset; // 플레이어와 카메라 사이의 거리
    private Vector3 initialCameraPosition; // 카메라의 초기 위치

    void Start()
    {
        // 플레이어 오브젝트를 찾아서 설정
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        offset = transform.position - player.position;
        initialCameraPosition = transform.position;
    }

    void Update()
    {
        // WASD 키 입력에 의한 카메라 이동
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);
        transform.position += movement * speed * Time.deltaTime;

        // 마우스 위치에 따른 카메라 이동
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.x > Screen.width - edgeSize && transform.position.x < initialCameraPosition.x + maxMouseEdgeDistance)
        {
            transform.position += Vector3.right * mouseEdgeScrollSpeed * Time.deltaTime;
        }
        else if (mousePosition.x < edgeSize && transform.position.x > initialCameraPosition.x - maxMouseEdgeDistance)
        {
            transform.position += Vector3.left * mouseEdgeScrollSpeed * Time.deltaTime;
        }
        if (mousePosition.y > Screen.height - edgeSize && transform.position.y < initialCameraPosition.y + maxMouseEdgeDistance)
        {
            transform.position += Vector3.up * mouseEdgeScrollSpeed * Time.deltaTime;
        }
        else if (mousePosition.y < edgeSize && transform.position.y > initialCameraPosition.y - maxMouseEdgeDistance)
        {
            transform.position += Vector3.down * mouseEdgeScrollSpeed * Time.deltaTime;
        }

        // 마우스가 화면 중앙으로 돌아오면 플레이어 중심으로 카메라 이동
        if (mousePosition.x >= edgeSize && mousePosition.x <= Screen.width - edgeSize &&
            mousePosition.y >= edgeSize && mousePosition.y <= Screen.height - edgeSize)
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }
}
