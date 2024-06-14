using System.Collections;
using UnityEngine;

public class NPCCombatAI : MonoBehaviour, ICombatant
{
    public GameObject projectilePrefab; // 투사체 Prefab
    public float projectileSpeed = 5f; // 투사체 속도
    public int projectileDamage = 10; // 투사체 데미지
    public int maxProjectilesPerTurn = 3; // 턴당 최대 발사체 수
    private int projectilesFiredThisTurn = 0; // 현재 턴에서 발사한 발사체 수
    private int projectilesOnField = 0; // 필드에 존재하는 발사체 수
    public int maxActionPoints = 5; // 최대 행동력
    private int currentActionPoints;
    private bool isTurnComplete; // 이 줄을 추가합니다.
    private Rigidbody2D rb;
    public LayerMask coverLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator ExecuteCombatAI(int actionPoints)
    {
        // 엄폐물을 찾아 이동
        MoveToCover(actionPoints);
        yield return new WaitForSeconds(1.0f);

        // 플레이어 공격
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Attack(direction);
            yield return new WaitForSeconds(1.0f);
        }

        EndTurn(); // 공격 후 턴 종료
    }

    private void MoveToCover(int actionPoints)
    {
        Collider2D[] coverObjects = Physics2D.OverlapCircleAll(transform.position, 5.0f, coverLayer);
        if (coverObjects.Length > 0)
        {
            Collider2D nearestCover = coverObjects[0];
            float nearestDistance = Vector2.Distance(transform.position, coverObjects[0].transform.position);

            foreach (var cover in coverObjects)
            {
                float distance = Vector2.Distance(transform.position, cover.transform.position);
                if (distance < nearestDistance)
                {
                    nearestCover = cover;
                    nearestDistance = distance;
                }
            }

            if (nearestCover != null)
            {
                Vector2 direction = (nearestCover.transform.position - transform.position).normalized;
                MoveCharacter(direction, actionPoints);
            }
        }
    }

    private void MoveCharacter(Vector2 direction, int actionPoints)
    {
        Vector2 newPosition = rb.position + direction * 3f * Time.fixedDeltaTime;
        float distance = Vector2.Distance(rb.position, newPosition);

        if (distance > 0)
        {
            rb.MovePosition(newPosition);
        }
    }

    public void Attack(Vector2 direction)
    {
        if (projectilesFiredThisTurn >= maxProjectilesPerTurn)
            return;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb == null)
        {
            projectileRb = projectile.AddComponent<Rigidbody2D>();
        }
        projectileRb.isKinematic = true; // 발사체를 Kinematic으로 설정
        projectileRb.velocity = direction * projectileSpeed; // 발사체 속도 설정

        // 발사체와 NPC의 충돌 무시
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // 발사체에 충돌 핸들러 추가 및 데미지 설정
        ProjectileCollisionHandler collisionHandler = projectile.AddComponent<ProjectileCollisionHandler>();
        collisionHandler.damage = projectileDamage;
        collisionHandler.projectileOwner = this;

        projectilesFiredThisTurn++; // 현재 턴에서 발사한 발사체 수 증가
        projectilesOnField++; // 필드에 존재하는 발사체 수 증가
        StartCoroutine(DestroyProjectileAfterTime(projectile, 5f)); // 5초 후에 발사체 제거
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

    public void NotifyProjectileDestroyed()
    {
        projectilesOnField--; // 필드에 존재하는 발사체 수 감소
    }

    public void ResetProjectilesFired()
    {
        projectilesFiredThisTurn = 0; // 발사체 수 초기화
    }

    public void StartTurn()
    {
        currentActionPoints = maxActionPoints; // 턴이 시작될 때 행동력 초기화
        isTurnComplete = false; // 턴 시작 시 초기화
        ResetProjectilesFired(); // 발사체 수 초기화
        StartCoroutine(ExecuteCombatAI(currentActionPoints));
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
