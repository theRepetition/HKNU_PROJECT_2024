using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool canMove = true;
    private PlayerTurnManager turnManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 0; // Rigidbody의 drag 설정을 0으로 유지
        turnManager = GetComponent<PlayerTurnManager>();
    }

    void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            MoveCharacter(movement);
        }
    }

    void HandleMovementInput()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        movement.Normalize(); // 대각선 움직임의 속도를 일정하게 유지
    }

    void MoveCharacter(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && turnManager.CurrentActionPoints > 0)
            {
                rb.velocity = direction * speed;
                turnManager.CurrentActionPoints--; // 이동할 때마다 행동력 소모
            }
            else if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.RealTime)
            {
                rb.velocity = direction * speed;
            }
        }
        else
        {
            rb.velocity = Vector2.zero; // 입력이 없을 경우 속도를 0으로 설정
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.velocity = Vector2.zero; // 움직임을 멈출 때 바로 멈추도록 설정
    }
}
