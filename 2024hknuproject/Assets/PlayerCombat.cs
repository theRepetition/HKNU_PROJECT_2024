using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject projectilePrefab; // 투사체 Prefab
    public int maxProjectilesPerTurn = 3; // 턴당 최대 발사체 수
    public int projectileDamage = 10; // 발사체 데미지
    public float projectileLifetime = 5f; // 발사체가 사라지는 시간
    private int projectilesFiredThisTurn = 0; // 현재 턴에서 발사한 발사체 수
    private int projectilesOnField = 0; // 필드에 존재하는 발사체 수
    private LineRenderer lineRenderer;
    private Vector2 aimDirection;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // 초기에는 경로를 표시하지 않음

        // LineRenderer가 타일 위에 보이도록 Z축 조정
        lineRenderer.sortingOrder = 1;
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
                ShootProjectile();
            }
        }
        else if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.RealTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShowAim();
            }

            if (Input.GetMouseButtonUp(0))
            {
                ShootProjectile();
                lineRenderer.positionCount = 0; // 경로 숨기기
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

        // 발사체에 충돌 핸들러 추가 및 데미지 설정
        ProjectileCollisionHandler collisionHandler = projectile.AddComponent<ProjectileCollisionHandler>();
        collisionHandler.damage = projectileDamage;
        collisionHandler.projectileOwner = this; // 발사체 소유자 설정

        projectilesFiredThisTurn++; // 현재 턴에서 발사한 발사체 수 증가
        projectilesOnField++; // 필드에 존재하는 발사체 수 증가
        StartCoroutine(DestroyProjectileAfterTime(projectile, projectileLifetime)); // 설정된 시간 후에 발사체 제거
    }

    IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        if (projectile != null)
        {
            Destroy(projectile);
            projectilesOnField--; // 필드에 존재하는 발사체 수 감소
        }
    }

    public void ResetProjectilesFired()
    {
        projectilesFiredThisTurn = 0; // 발사체 수 초기화
    }

    public void NotifyProjectileDestroyed()
    {
        projectilesOnField--; // 필드에 존재하는 발사체 수 감소
    }

    public int ProjectilesFiredThisTurn => projectilesFiredThisTurn; // 발사체 수 반환

    public int ProjectilesOnField => projectilesOnField; // 필드에 존재하는 발사체 수 반환
}
