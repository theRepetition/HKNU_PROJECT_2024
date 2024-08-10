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
        if (movement != Vector2.zero)
        {
            if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && turnManager.CurrentActionPoints > 0)
            {
                MoveCharacter(movement);
                turnManager.CurrentActionPoints--; // 이동할 때마다 행동력 소모
            }
            else if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.RealTime)
            {
                MoveCharacter(movement);
            }
        }
    }

    void HandleMovementInput()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }

    void MoveCharacter(Vector2 direction)
    {
        Vector2 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        movement = Vector2.zero;
    }
}
