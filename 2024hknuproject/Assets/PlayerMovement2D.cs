using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour, ITurnTaker
{
    public float speed = 5.0f;
    public int maxActionPoints = 10; // �ִ� �ൿ��
    public GameObject projectilePrefab; // ����ü Prefab
    private int currentActionPoints;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool canMove = true;
    private bool isTurnComplete = false;
    private LineRenderer lineRenderer;
    private Vector2 aimDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // �ʱ⿡�� ��θ� ǥ������ ����

        // LineRenderer�� Ÿ�� ���� ���̵��� Z�� ����
        lineRenderer.sortingOrder = 1;

        TurnManager.Instance.RegisterTurnTaker(this);
    }

    void Update()
    {
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            if (TurnManager.Instance.CurrentTurnTaker == this)
            {
                HandleTurnBasedMovement();
                HandleTurnBasedCombat();
            }
        }
        else
        {
            HandleRealTimeMovement();
        }
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero && canMove)
        {
            MoveCharacter(movement);
        }
    }

    void HandleRealTimeMovement()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }

    void HandleTurnBasedMovement()
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

        if (Input.GetKeyDown(KeyCode.Space) && !isTurnComplete)
        {
            Debug.Log("�����̽��� �Է� ����: �� ����");
            EndTurn();
        }
    }

    void HandleTurnBasedCombat()
    {
        if (Input.GetMouseButton(0) && !isTurnComplete) // ���콺 ��Ŭ�� ����
        {
            ShowAim();
        }

        if (Input.GetMouseButtonUp(0) && !isTurnComplete) // ���콺 ��Ŭ�� ����
        {
            ShootProjectile();
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

    void ShowAim()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimDirection = (mousePosition - transform.position).normalized;

        // ��� ǥ��
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + (Vector3)(aimDirection * 10f)); // ������ ���� ����

        // LineRenderer�� Ÿ�� ���� ���̵��� Z�� ����
        lineRenderer.SetPosition(0, new Vector3(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(0).y, -1));
        lineRenderer.SetPosition(1, new Vector3(lineRenderer.GetPosition(1).x, lineRenderer.GetPosition(1).y, -1));
    }

    void ShootProjectile()
    {
        lineRenderer.positionCount = 0; // ��� �����

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb == null)
        {
            projectileRb = projectile.AddComponent<Rigidbody2D>();
        }
        projectileRb.isKinematic = true; // �߻�ü�� Kinematic���� ����
        projectileRb.velocity = aimDirection * 5f; // �߻�ü �ӵ� ���� (������ ������ ����)

        // �߻�ü�� �÷��̾��� �浹 ����
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // �߻�ü�� �浹 �ڵ鷯 �߰�
        projectile.AddComponent<ProjectileCollisionHandler>();
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

    public void StartTurn()
    {
        currentActionPoints = maxActionPoints; // ���� ���۵� �� �ൿ�� �ʱ�ȭ
        isTurnComplete = false; // �� ���� �� �ʱ�ȭ
        EnableMovement();
        Debug.Log("�� �� ����");
    }

    public void EndTurn()
    {
        DisableMovement();
        isTurnComplete = true; // �� �Ϸ� ����
        Debug.Log("�� ����");
        TurnManager.Instance.NextTurn(); // �� ���� �� ���� ������ ��ȯ
    }

    public bool IsTurnComplete => isTurnComplete; // �� �Ϸ� ����

    public string Name => gameObject.name; // �̸� ��ȯ
}
