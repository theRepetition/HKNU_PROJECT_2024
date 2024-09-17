using System.Collections;
using UnityEngine;

public class NPCCombatAI : MonoBehaviour, ICombatant
{
    public GameObject projectilePrefab; // 발사체 프리팹
    public float projectileSpeed = 5f; // 발사체 속도
    public int projectileDamage = 10; // 발사체 피해량
    public int maxProjectilesPerTurn = 3; // 턴당 최대 발사체 수
    private int projectilesFiredThisTurn = 0; // 이번 턴에 발사된 발사체 수
    private int projectilesOnField = 0; // 필드에 남아있는 발사체 수
    public int maxActionPoints = 5; // 최대 행동 포인트
    private int currentActionPoints; // 현재 남은 행동 포인트
    private bool isTurnComplete; // 턴이 완료되었는지 여부
    private Rigidbody2D rb; // NPC의 Rigidbody2D 컴포넌트
    public LayerMask coverLayer; // 엄폐물 레이어
    public float weaponRange = 10f; // 무기 사거리
    public float moveSpeed = 2f; // 이동 속도

    private Vector2 targetPosition; // 목표 위치
    private bool isMoving; // 이동 중인지 여부
    private float timeAtSamePosition; // 동일한 위치에 머문 시간
    private Vector2 lastPosition; // 마지막 위치

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
    }

    private void FixedUpdate()
    {
        // NPC가 이동 중일 때
        if (isMoving && !isTurnComplete) // 턴이 종료된 상태가 아닐 때만 이동 처리
        {
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            if (Vector2.Distance(rb.position, targetPosition) < 0.1f) // 목표 위치에 도달하면 이동 종료
            {
                isMoving = false;
            }

            if (rb.position == lastPosition) // NPC가 같은 위치에 머무는지 확인
            {
                timeAtSamePosition += Time.fixedDeltaTime;
                if (timeAtSamePosition >= 3.0f) // 3초 이상 같은 위치에 있으면 턴 종료
                {
                    isMoving = false;
                    if (!isTurnComplete) // 턴이 종료되지 않았을 경우에만 EndTurn() 호출
                    {
                        EndTurn();
                    }
                }
            }
            else
            {
                lastPosition = rb.position;
                timeAtSamePosition = 0f;
            }
        }
    }

    // NPC의 전투 AI를 실행하는 코루틴
    public IEnumerator ExecuteCombatAI(int actionPoints)
    {
        if (isTurnComplete) yield break; // 턴이 이미 끝났다면 더 이상 진행하지 않음

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(rb.position, player.transform.position);

            // 플레이어와의 거리가 무기 사거리 밖이면 이동
            if (distanceToPlayer > weaponRange)
            {
                MoveToPosition((Vector2)player.transform.position);
                yield return new WaitUntil(() => !isMoving); // 이동이 끝날 때까지 대기
            }

            yield return new WaitForSeconds(1.0f);

            // 플레이어 공격
            Attack(directionToPlayer);
            yield return new WaitForSeconds(1.0f);
        }

        // 엄폐물을 찾고 이동
        FindBestCoverAndMove();

        yield return new WaitForSeconds(3.0f); // NPC는 3초 동안 대기

        if (!isTurnComplete) // 턴이 종료되지 않았을 경우에만 EndTurn() 호출
        {
            EndTurn();
        }
    }

    // 가장 적합한 엄폐물을 찾아 이동하는 함수
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

            // 엄폐물이 플레이어와 NPC 사이에 있는지 확인
            float dotProduct = Vector2.Dot(directionToCover, directionToPlayerFromCover);
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

    // 최적의 엄폐물 위치를 계산하는 함수
    private Vector2 CalculateOptimalPosition(Vector2 coverPosition, Vector2 playerPosition)
    {
        Vector2 directionToPlayer = (playerPosition - coverPosition).normalized;
        Vector2 optimalPosition = coverPosition - directionToPlayer * 1.0f; // 엄폐물에서 플레이어와 1.0f 떨어진 위치로 이동
        return optimalPosition;
    }

    // NPC를 목표 위치로 이동시키는 함수
    private void MoveToPosition(Vector2 position)
    {
        targetPosition = position;
        isMoving = true;
    }

    // NPC 공격 함수
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

        // NPC와 발사체의 충돌 무시
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // 발사체 충돌 핸들러 추가 및 피해량 설정
        ProjectileCollisionHandler collisionHandler = projectile.AddComponent<ProjectileCollisionHandler>();
        collisionHandler.damage = projectileDamage;
        collisionHandler.projectileOwner = this;

        projectilesFiredThisTurn++; // 이번 턴에 발사된 발사체 수 증가
        projectilesOnField++; // 필드에 있는 발사체 수 증가
        StartCoroutine(DestroyProjectileAfterTime(projectile, 5f)); // 5초 후 발사체 파괴
    }

    // 발사체를 일정 시간이 지난 후 파괴하는 코루틴
    IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        if (projectile != null)
        {
            Destroy(projectile);
            projectilesOnField--; // 필드에서 발사체 제거
        }
    }

    // 발사체가 파괴되었음을 알리는 함수
    public void NotifyProjectileDestroyed()
    {
        projectilesOnField--; // 필드에서 발사체 수 감소
    }

    // 턴이 시작될 때 발사체 수 초기화
    public void ResetProjectilesFired()
    {
        projectilesFiredThisTurn = 0;
    }

    // NPC의 턴 시작
    public void StartTurn()
    {
        currentActionPoints = maxActionPoints; // 행동 포인트 초기화
        isTurnComplete = false; // 턴 완료 상태 초기화
        ResetProjectilesFired(); // 발사체 수 초기화
        timeAtSamePosition = 0f;
        lastPosition = rb.position;
        StartCoroutine(ExecuteCombatAI(currentActionPoints)); // 전투 AI 실행
    }

    // NPC의 턴 종료
    public void EndTurn()
    {
        if (isTurnComplete) return; // 이미 턴이 종료된 경우 실행하지 않음

        isTurnComplete = true; // 턴 종료 상태로 변경
        TurnManager.Instance.NextTurn(); // 다음 턴으로 넘어감
        Debug.Log("Turn ended.");
    }

    // ICombatant 인터페이스 구현
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
