using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour, ITurnTaker
{
    public float speed = 5.0f;
    public int maxActionPoints = 10; // �ִ� �ൿ��
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
                movement = Vector2.zero; // �̵��� �� ������ �������� ����
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
                currentActionPoints -= Mathf.CeilToInt(distance); // �ൿ�� �Ҹ�
            }
            else
            {
                movement = Vector2.zero; // �ൿ���� ������ ��� �������� ����
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
        currentActionPoints = maxActionPoints; // ���� ���۵� �� �ൿ�� �ʱ�ȭ
        EnableMovement();
        Debug.Log("�� �� ����");
        TurnManager.Instance.CheckForRealTimeMode();
    }

    public void EndTurn()
    {
        DisableMovement();
        TurnManager.Instance.NextTurn(); // �� ���� �� ���� ������ ��ȯ
    }

    public bool IsTurnComplete => !canMove; // canMove�� false�� �� ���� �Ϸ��

    public string Name => gameObject.name; // �̸� ��ȯ


}
