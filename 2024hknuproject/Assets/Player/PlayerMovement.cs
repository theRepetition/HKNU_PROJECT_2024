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
    public Animator animator; // Animator 컴포넌트

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
        movement.x = Input.GetAxisRaw("Horizontal"); // 수평 방향 입력 받기 (Raw로 변경해 미세한 움직임 방지)
        movement.y = Input.GetAxisRaw("Vertical"); // 수직 방향 입력 받기 (Raw로 변경해 미세한 움직임 방지)

        // 입력이 있을 때만 걷기 애니메이션 활성화
        if (movement != Vector2.zero)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
        // 캐릭터의 방향에 따라 스프라이트 반전
        if (movement.x > 0) // 왼쪽으로 이동할 때
        {
            transform.localScale = new Vector3(-2.3f, 2.3f, 1); // X 축을 -1로 설정하여 좌측 반전
        }
        else if (movement.x < 0) // 오른쪽으로 이동할 때
        {
            transform.localScale = new Vector3(2.3f, 2.3f, 1); // X 축을 1로 설정하여 우측 반전
        }
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
        Debug.Log($"이동가능@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@"); // 호출한 객체의 이름 출력
        Debug.Log("EnableMovement called. StackTrace: " + new System.Diagnostics.StackTrace());
        canMove = true; // 이동 가능 상태로 설정
    }

    // 플레이어 이동을 비활성화하는 함수
    public void DisableMovement()
    {
        Debug.Log($"이동불가!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"); // 호출한 객체의 이름 출력
        canMove = false; // 이동 불가능 상태로 설정
        movement = Vector2.zero; // 이동 벡터를 0으로 초기화하여 멈춤
        animator.SetBool("IsWalking", false); // 이동 멈출 때 애니메이션도 멈춤
    }
}
