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
    private bool isTurnComplete; // 턴 완료 확인
    private Rigidbody2D rb;
    public LayerMask coverLayer;
    public float weaponRange = 10f; // 무기의 사정거리
    public float moveSpeed = 2f; // 이동 속도

    private Vector2 targetPosition;
    private bool isMoving;
    private float timeAtSamePosition; // 동일 위치에 머문 시간
    private Vector2 lastPosition; // 마지막 위치

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }

            if (rb.position == lastPosition)
            {
                timeAtSamePosition += Time.fixedDeltaTime;
                if (timeAtSamePosition >= 3.0f)
                {
                    isMoving = false;
                    EndTurn(); // 3초 동안 움직임이 없으면 턴 종료
                }
            }
            else
            {
                lastPosition = rb.position;
                timeAtSamePosition = 0f;
            }
        }
    }

    public IEnumerator ExecuteCombatAI(int actionPoints)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(rb.position, player.transform.position);

            // 플레이어와의 거리를 계산하여 적절한 위치로 이동
            if (distanceToPlayer > weaponRange)
            {
                MoveToPosition((Vector2)player.transform.position); // player.transform.position을 Vector2로 변환
            }

            yield return new WaitForSeconds(1.0f);

            // 플레이어 공격
            Attack(directionToPlayer);
            yield return new WaitForSeconds(1.0f);
        }

        // 플레이어와 NPC 사이의 엄폐물 탐색 및 이동
        FindBestCoverAndMove();

        yield return new WaitForSeconds(3.0f); // NPC가 3초 동안 움직임이 없으면 턴 종료
        EndTurn();
    }

    private void FindBestCoverAndMove()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider2D[] coverObjects = Physics2D.OverlapCircleAll(rb.position, 5.0f, coverLayer);
        Collider2D bestCover = null;
        float bestCoverScore = float.MinValue;

        foreach (var cover in coverObjects)
        {
            Vector2 coverPosition = (Vector2)cover.transform.position;
            Vector2 directionToCover = (coverPosition - rb.position).normalized;
            Vector2 directionToPlayerFromCover = ((Vector2)player.transform.position - coverPosition).normalized;

            // 플레이어와 엄폐물이 NPC-엄폐물-플레이어로 일직선상에 있는지 확인
            float dotProduct = Vector2.Dot(directionToCover, directionToPlayerFromCover);

            // 일직선상에 가까울수록 점수를 높게, NPC와 엄폐물 사이의 거리로 점수 조정
            float distanceToNpc = Vector2.Distance(rb.position, coverPosition);
            float coverScore = dotProduct - distanceToNpc;

            if (coverScore > bestCoverScore)
            {
                bestCover = cover;
                bestCoverScore = coverScore;
            }
        }

        if (bestCover != null)
        {
            Vector2 optimalPosition = CalculateOptimalPosition(bestCover.transform.position, player.transform.position);
            MoveToPosition(optimalPosition);
        }
    }

    private Vector2 CalculateOptimalPosition(Vector2 coverPosition, Vector2 playerPosition)
    {
        Vector2 directionToPlayer = (playerPosition - coverPosition).normalized;
        Vector2 optimalPosition = coverPosition - directionToPlayer * 1.0f; // 엄폐물 뒤 1.0f 거리로 이동
        return optimalPosition;
    }

    private void MoveToPosition(Vector2 position)
    {
        targetPosition = position;
        isMoving = true;
    }

    private void MoveCharacter(Vector2 direction)
    {
        targetPosition = rb.position + direction * 3f;
        isMoving = true;
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
        timeAtSamePosition = 0f; // 시간을 초기화
        lastPosition = rb.position; // 현재 위치를 저장
        StartCoroutine(ExecuteCombatAI(currentActionPoints));
    }

    public void EndTurn()
    {
        isTurnComplete = true; // 턴 완료 설정
        TurnManager.Instance.NextTurn(); // 턴 종료 후 다음 턴으로 전환
        Debug.Log("Turn ended.");
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
