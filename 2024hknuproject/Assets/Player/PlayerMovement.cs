using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f; // 플레이어 이동 속도
    private Rigidbody2D rb; // 플레이어의 Rigidbody2D 컴포넌트
    private Vector2 movement; // 이동 방향 벡터
    private bool canMove = true; // 플레이어가 이동 가능한 상태인지 여부
    private PlayerTurnManager turnManager; // 플레이어의 턴을 관리하는 스크립트

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트를 가져옴
        turnManager = GetComponent<PlayerTurnManager>(); // PlayerTurnManager 스크립트를 가져옴
    }

    void Update()
    {
        if (canMove) // 플레이어가 이동 가능한 상태일 때
        {
            HandleMovementInput(); // 입력 처리
        }
    }

    void FixedUpdate()
    {
        // 이동 방향이 입력되었을 때
        if (movement != Vector2.zero)
        {
            // 턴제 모드일 때
            if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && turnManager.CurrentActionPoints > 0)
            {
                MoveCharacter(movement); // 캐릭터 이동
                turnManager.CurrentActionPoints--; // 이동 시 행동 포인트 감소
            }
            // 실시간 모드일 때
            else if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.RealTime)
            {
                MoveCharacter(movement); // 캐릭터 이동
            }
        }
    }

    // 플레이어의 입력을 처리하는 함수
    void HandleMovementInput()
    {
        movement.x = Input.GetAxis("Horizontal"); // 수평 방향 입력 받기
        movement.y = Input.GetAxis("Vertical"); // 수직 방향 입력 받기
    }

    // 캐릭터를 이동시키는 함수
    void MoveCharacter(Vector2 direction)
    {
        Vector2 newPosition = rb.position + direction * speed * Time.fixedDeltaTime; // 새 위치 계산
        rb.MovePosition(newPosition); // Rigidbody2D를 사용해 이동
    }

    // 플레이어 이동을 활성화하는 함수
    public void EnableMovement()
    {
        canMove = true; // 이동 가능 상태로 설정
    }

    // 플레이어 이동을 비활성화하는 함수
    public void DisableMovement()
    {
        canMove = false; // 이동 불가능 상태로 설정
        movement = Vector2.zero; // 이동 벡터를 0으로 초기화하여 멈춤
    }
}
