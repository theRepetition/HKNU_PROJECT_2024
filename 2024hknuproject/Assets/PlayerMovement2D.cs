using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour, ITurnTaker
{
    public float speed = 5.0f;
    public int maxActionPoints = 10; // 최대 행동력
    public GameObject projectilePrefab; // 투사체 Prefab
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
        lineRenderer.positionCount = 0; // 초기에는 경로를 표시하지 않음

        // LineRenderer가 타일 위에 보이도록 Z축 조정
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
            movement = Vector2.zero; // 이동할 수 없으면 움직임을 막음
        }
        else
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isTurnComplete)
        {
            Debug.Log("스페이스바 입력 감지: 턴 종료");
            EndTurn();
        }
    }

    void HandleTurnBasedCombat()
    {
        if (Input.GetMouseButton(0) && !isTurnComplete) // 마우스 좌클릭 유지
        {
            ShowAim();
        }

        if (Input.GetMouseButtonUp(0) && !isTurnComplete) // 마우스 좌클릭 해제
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

    void ShowAim()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimDirection = (mousePosition - transform.position).normalized;

        // 경로 표시
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + (Vector3)(aimDirection * 10f)); // 임의의 길이 설정

        // LineRenderer가 타일 위에 보이도록 Z축 조정
        lineRenderer.SetPosition(0, new Vector3(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(0).y, -1));
        lineRenderer.SetPosition(1, new Vector3(lineRenderer.GetPosition(1).x, lineRenderer.GetPosition(1).y, -1));
    }

    void ShootProjectile()
    {
        lineRenderer.positionCount = 0; // 경로 숨기기

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb == null)
        {
            projectileRb = projectile.AddComponent<Rigidbody2D>();
        }
        projectileRb.isKinematic = true; // 발사체를 Kinematic으로 설정
        projectileRb.velocity = aimDirection * 5f; // 발사체 속도 설정 (적절한 값으로 조정)

        // 발사체와 플레이어의 충돌 무시
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // 발사체에 충돌 핸들러 추가
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
        currentActionPoints = maxActionPoints; // 턴이 시작될 때 행동력 초기화
        isTurnComplete = false; // 턴 시작 시 초기화
        EnableMovement();
        Debug.Log("내 턴 시작");
    }

    public void EndTurn()
    {
        DisableMovement();
        isTurnComplete = true; // 턴 완료 설정
        Debug.Log("턴 종료");
        TurnManager.Instance.NextTurn(); // 턴 종료 후 다음 턴으로 전환
    }

    public bool IsTurnComplete => isTurnComplete; // 턴 완료 여부

    public string Name => gameObject.name; // 이름 반환
}
