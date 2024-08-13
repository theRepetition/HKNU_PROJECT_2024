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
        rb.drag = 0; // Rigidbody�� drag ������ 0���� ����
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
        movement.Normalize(); // �밢�� �������� �ӵ��� �����ϰ� ����
    }

    void MoveCharacter(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && turnManager.CurrentActionPoints > 0)
            {
                rb.velocity = direction * speed;
                turnManager.CurrentActionPoints--; // �̵��� ������ �ൿ�� �Ҹ�
            }
            else if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.RealTime)
            {
                rb.velocity = direction * speed;
            }
        }
        else
        {
            rb.velocity = Vector2.zero; // �Է��� ���� ��� �ӵ��� 0���� ����
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.velocity = Vector2.zero; // �������� ���� �� �ٷ� ���ߵ��� ����
    }
}
