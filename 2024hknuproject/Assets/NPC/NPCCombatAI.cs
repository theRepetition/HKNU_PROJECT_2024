using System.Collections;
using UnityEngine;

public class NPCCombatAI : MonoBehaviour, ICombatant
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public int projectileDamage = 10;
    public int maxProjectilesPerTurn = 3;
    private int projectilesFiredThisTurn = 0;
    private int projectilesOnField = 0;
    public int maxActionPoints = 5;
    private int currentActionPoints;
    private bool isTurnComplete;
    private Rigidbody2D rb;
    public LayerMask coverLayer; // 엄폐물 레이어
    public float weaponRange = 10f; // 무기 사거리
    public float moveSpeed = 2f;

    private Vector2 targetPosition;
    private bool isMoving;
    private float timeAtSamePosition;
    private Vector2 lastPosition;
    private Collider2D currentCover; // NPC가 숨을 엄폐물
    private Vector2 coverPosition; // 엄폐물 위치 저장
    private GameObject player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (isMoving && !isTurnComplete)
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
                    if (!isTurnComplete)
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

    public IEnumerator ExecuteCombatAI(int actionPoints)
    {
        if (isTurnComplete) yield break;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(rb.position, player.transform.position);

            // NPC와 플레이어 사이에 장애물이 있는지 확인
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, coverLayer);
            if (hit.collider != null)
            {
                Debug.Log("NPC와 플레이어 사이에 장애물이 있음. 벽을 따라 수직 이동합니다.");

                // 벽을 따라 수직으로 이동
                yield return StartCoroutine(MoveVerticallyAlongWall(directionToPlayer, distanceToPlayer));
                Attack(directionToPlayer); // 플레이어 공격
            }
            else
            {
                // 장애물이 없을 경우 플레이어를 공격
                yield return new WaitForSeconds(1.0f); // 약간의 대기 시간
                Attack(directionToPlayer); // 플레이어 공격
                yield return new WaitForSeconds(1.0f); // 공격 후 대기 시간
            }
        }

        // 엄폐물을 찾고 이동하는 로직 실행
        FindBestCoverAndMove();

        yield return new WaitForSeconds(3.0f); // NPC는 3초 동안 대기

        if (!isTurnComplete)
        {
            EndTurn(); // 턴 종료
        }
    }




    // NPC가 이동할 때 장애물 탐색 및 회피 로직
    // 벽을 따라 수직으로 이동하는 함수
    private IEnumerator MoveVerticallyAlongWall(Vector2 directionToPlayer, float distanceToPlayer)
    {
        Vector2 initialPosition = rb.position;
        float movementStep = 0.2f; // 한 번에 이동할 거리
        float maxDistance = 5.0f; // 최대 이동 거리
        float distanceMoved = 0.0f;

        // 벽을 따라 수직 이동
        Vector2 wallVerticalDirection = Vector2.Perpendicular(directionToPlayer); // 플레이어 방향의 수직 벡터

        bool moveUp = true; // 벽을 따라 위로 이동할지 아래로 이동할지 결정하는 플래그
        int attempts = 0; // 회피 시도 횟수 제한

        // 벽을 따라 수직으로 이동하며 목표 위치에 장애물이 없는지 확인
        while (distanceMoved < maxDistance)
        {
            // 위쪽 또는 아래쪽으로 이동 시도
            Vector2 nextPosition = moveUp
                ? initialPosition + wallVerticalDirection * movementStep
                : initialPosition - wallVerticalDirection * movementStep;

            // 이동을 시도
            targetPosition = nextPosition;
            isMoving = true;

            // 이동이 끝날 때까지 대기
            yield return new WaitUntil(() => !isMoving);

            // 이동 후 NPC와 플레이어 사이에 장애물이 없는지 다시 확인
            RaycastHit2D hit = Physics2D.Raycast(rb.position, directionToPlayer, distanceToPlayer, coverLayer);
            if (hit.collider == null)
            {
                Debug.Log("장애물이 없어졌습니다. 목표 위치로 이동합니다.");
                MoveToPosition(targetPosition); // 장애물이 없으면 목표 위치로 이동
                yield break; // 이동 후 종료
            }

            // 이동하려는 방향에 또 다른 장애물이 있으면 방향 전환
            RaycastHit2D hitInMoveDirection = Physics2D.Raycast(rb.position, wallVerticalDirection, movementStep, coverLayer);
            if (hitInMoveDirection.collider != null)
            {
                // 방향을 반대로
                Debug.Log("이동 방향에 또 다른 장애물이 있습니다. 방향 전환합니다.");
                moveUp = !moveUp;
                attempts++;
                if (attempts > 4)
                {
                    Debug.Log("회피 시도 초과. 더 이상 이동하지 않음.");
                    yield break; // 너무 많은 시도는 중지
                }
            }

            // 계속 수직으로 이동
            distanceMoved += movementStep;
            initialPosition = rb.position;
        }

        Debug.Log("최대 이동 거리까지 이동했지만 장애물이 남아 있습니다.");
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
            // bestCover의 Collider2D를 인자로 전달
            Vector2 optimalPosition = CalculateOptimalPosition(bestCover.transform.position, player.transform.position, bestCover);
            MoveToPosition(optimalPosition);
        }
    }


    // 최적의 엄폐물 위치를 계산하는 함수
    private Vector2 CalculateOptimalPosition(Vector2 coverPosition, Vector2 playerPosition, Collider2D coverCollider)
    {
        Vector2 directionToPlayer = (playerPosition - coverPosition).normalized;

        // 엄폐물의 크기(반경)를 고려하여 경계까지의 거리 계산
        float coverRadius = coverCollider.bounds.extents.magnitude;

        // 엄폐물에서 일정 거리 유지 (safeDistance 만큼 떨어진 위치로 이동)
        float safeDistance = 2.0f; // 엄폐물과의 거리
        Vector2 optimalPosition = coverPosition - directionToPlayer * (coverRadius + safeDistance);

        return optimalPosition;
    }



    // NPC를 목표 위치로 이동시키는 함수 (회피 탐색 추가)
    private void MoveToPosition(Vector2 position)
    {
        // NPC와 목표 위치 사이에 장애물이 있는지 확인
        RaycastHit2D hit = Physics2D.Raycast(rb.position, (position - rb.position).normalized, Vector2.Distance(rb.position, position), coverLayer);

        if (hit.collider != null)
        {
            // 장애물이 있을 경우 회피 탐색 로직 시작
            Debug.Log("경로에 장애물이 있습니다: " + hit.collider.name);

            // 벽을 따라 수직으로 이동하는 로직 실행
            StartCoroutine(MoveVerticallyAlongWall((position - rb.position).normalized, Vector2.Distance(rb.position, position)));
            return;
        }

        // 장애물이 없을 때 이동
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

        projectilesFiredThisTurn++;
        projectilesOnField++;
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

    public void NotifyProjectileDestroyed()
    {
        projectilesOnField--;
    }

    public void ResetProjectilesFired()
    {
        projectilesFiredThisTurn = 0;
    }

    // NPC의 턴 시작
    public void StartTurn()
    {
        currentActionPoints = maxActionPoints;
        isTurnComplete = false;
        ResetProjectilesFired();
        timeAtSamePosition = 0f;
        lastPosition = rb.position;
        StartCoroutine(ExecuteCombatAI(currentActionPoints)); // 전투 AI 실행
    }

    // NPC의 턴 종료
    public void EndTurn()
    {
        if (isTurnComplete) return;

        isTurnComplete = true;
        TurnManager.Instance.NextTurn();
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
