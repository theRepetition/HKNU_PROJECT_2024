using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ICombatant
{
    public GameObject projectilePrefab; // 투사체 Prefab
    public float projectileSpeed = 5f; // 투사체 속도
    public int projectileDamage = 10; // 투사체 데미지
    public int maxProjectilesPerTurn = 3; // 턴당 최대 발사체 수
    private int projectilesFiredThisTurn = 0; // 현재 턴에서 발사한 발사체 수
    private int projectilesOnField = 0; // 필드에 존재하는 발사체 수
    public int maxActionPoints = 10; // 최대 행동력
    private int currentActionPoints;
    private LineRenderer lineRenderer;
    private Vector2 aimDirection;
    private bool isTurnComplete = false;

    private HUDManager hudManager; // HUDManager 참조

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // 초기에는 경로를 표시하지 않음

        // LineRenderer가 타일 위에 보이도록 Z축 조정
        lineRenderer.sortingOrder = 1;

        hudManager = FindObjectOfType<HUDManager>(); // HUDManager 찾기
        if (hudManager != null)
        {
            hudManager.UpdateBulletSlots(maxProjectilesPerTurn - projectilesFiredThisTurn); // HUD 초기화
        }
    }

    void Update()
    {
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && TurnManager.Instance.CurrentTurnTaker == GetComponent<PlayerTurnManager>())
        {
            if (Input.GetMouseButton(0) && projectilesFiredThisTurn < maxProjectilesPerTurn) // 마우스 좌클릭 유지
            {
                ShowAim();
            }

            if (Input.GetMouseButtonUp(0) && projectilesFiredThisTurn < maxProjectilesPerTurn) // 마우스 좌클릭 해제
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (mousePosition - transform.position).normalized;
                Attack(direction);
            }

            if (Input.GetKeyDown(KeyCode.Space) && !isTurnComplete && projectilesOnField == 0)
            {
                EndTurn();
            }
        }
        else if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.RealTime)
        {
            if (Input.GetMouseButtonDown(0) && projectilesFiredThisTurn < maxProjectilesPerTurn)
            {
                ShowAim();
            }

            if (Input.GetMouseButtonUp(0) && projectilesFiredThisTurn < maxProjectilesPerTurn)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (mousePosition - transform.position).normalized;
                Attack(direction);
                lineRenderer.positionCount = 0; // 경로 숨기기
            }

            if (Input.GetKeyDown(KeyCode.R)) // 재장전 키 (예: R 키)
            {
                Reload();
            }
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

    public void Attack(Vector2 direction)
    {
        if (projectilesFiredThisTurn >= maxProjectilesPerTurn) // 탄환이 다 소모되면 공격 불가
            return;

        lineRenderer.positionCount = 0; // 경로 숨기기

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb == null)
        {
            projectileRb = projectile.AddComponent<Rigidbody2D>();
        }
        projectileRb.isKinematic = true; // 발사체를 Kinematic으로 설정
        projectileRb.velocity = direction * projectileSpeed; // 발사체 속도 설정

        // 발사체와 플레이어의 충돌 무시
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // 발사체에 충돌 핸들러 추가 및 데미지 설정
        ProjectileCollisionHandler collisionHandler = projectile.AddComponent<ProjectileCollisionHandler>();
        collisionHandler.damage = projectileDamage;
        collisionHandler.projectileOwner = this;

        projectilesFiredThisTurn++; // 현재 턴에서 발사한 발사체 수 증가
        projectilesOnField++; // 필드에 존재하는 발사체 수 증가

        hudManager.UpdateBulletSlots(maxProjectilesPerTurn - projectilesFiredThisTurn); // HUD 업데이트

        StartCoroutine(DestroyProjectileAfterTime(projectile, 5f)); // 5초 후에 발사체 제거
    }

    IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        if (projectile != null)
        {
            Destroy(projectile);
            NotifyProjectileDestroyed();
        }
    }

    public void NotifyProjectileDestroyed()
    {
        projectilesOnField--; // 필드에 존재하는 발사체 수 감소
    }

    public void ResetProjectilesFired()
    {
        projectilesFiredThisTurn = 0; // 발사체 수 초기화
        hudManager.UpdateBulletSlots(maxProjectilesPerTurn - projectilesFiredThisTurn); // HUD 업데이트
    }

    public void Reload()
    {
        projectilesFiredThisTurn = 0; // 발사체 수 초기화
        Debug.Log("재장전 완료");

        hudManager.UpdateBulletSlots(maxProjectilesPerTurn); // HUD 업데이트

        // 턴제 모드일 때 재장전하면 턴 종료
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            EndTurn();
        }
    }

    public void StartTurn()
    {
        isTurnComplete = false; // 턴 시작 시 초기화
        ResetProjectilesFired(); // 발사체 수 초기화
    }

    public void EndTurn()
    {
        isTurnComplete = true; // 턴 완료 설정
        TurnManager.Instance.NextTurn(); // 턴 종료 후 다음 턴으로 전환
    }

    public int MaxActionPoints => maxActionPoints;
    public int CurrentActionPoints
    {
        get => currentActionPoints;
        set => currentActionPoints = value;
    }

    public int MaxProjectilesPerTurn => maxProjectilesPerTurn;
    public int ProjectilesFiredThisTurn
    {
        get => projectilesFiredThisTurn;
        set => projectilesFiredThisTurn = value;
    }

    public GameObject ProjectilePrefab => projectilePrefab;
    public float ProjectileSpeed => projectileSpeed;
    public int ProjectileDamage => projectileDamage;

    public int ProjectilesOnField
    {
        get => projectilesOnField;
        set => projectilesOnField = value;
    }
}
