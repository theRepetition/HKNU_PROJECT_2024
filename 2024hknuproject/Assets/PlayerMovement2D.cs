using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour, ITurnTaker
{
    public float speed = 5.0f;
    public int maxActionPoints = 10; // 최대 행동력
    private int currentActionPoints;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        TurnManager.Instance.RegisterTurnTaker(this);
    }

    void Update()
    {
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            if (!canMove || currentActionPoints <= 0)
            {
                movement = Vector2.zero; // 이동할 수 없으면 움직임을 막음
            }
            else
            {
                movement.x = Input.GetAxis("Horizontal");
                movement.y = Input.GetAxis("Vertical");
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndTurn();
            }
        }
        else
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
        }
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            MoveCharacter(movement);
        }
    }

    void MoveCharacter(Vector2 direction)
    {
        Vector2 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        float distance = Vector2.Distance(rb.position, newPosition);

        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && currentActionPoints > 0)
        {
            if (currentActionPoints >= distance)
            {
                rb.MovePosition(newPosition);
                currentActionPoints -= Mathf.CeilToInt(distance); // 행동력 소모
            }
            else
            {
                movement = Vector2.zero; // 행동력이 부족할 경우 움직임을 멈춤
            }
        }
        else
        {
            rb.MovePosition(newPosition);
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void StartTurn()

    {      
        currentActionPoints = maxActionPoints; // 턴이 시작될 때 행동력 초기화
        EnableMovement();
        Debug.Log("내 턴 시작");
        TurnManager.Instance.CheckForRealTimeMode();
    }

    public void EndTurn()
    {
        DisableMovement();
        TurnManager.Instance.NextTurn(); // 턴 종료 후 다음 턴으로 전환
    }

    public bool IsTurnComplete => !canMove; // canMove가 false일 때 턴이 완료됨

    public string Name => gameObject.name; // 이름 반환


}
