using System.Collections;
using UnityEngine;
using Pathfinding;

public class NPCCombatAI : MonoBehaviour, ICombatant
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public int projectileDamage = 10;
    public int maxProjectilesPerTurn = 3;
    public float weaponRange = 5f;
    public float moveSpeed = 2f;
    public LayerMask coverLayer;

    private bool isTurnComplete = true;
    private bool isMoving;
    private GameObject player;
    private AIPath aiPath;
    private Seeker seeker;
    private Rigidbody2D rb; 
    public float pushForce = 15f; // 상대를 밀어내는 힘의 크기
    public float pauseDuration = 5f;
    public float slideFriction = 0.8f;  // 벽에 부딪혔을 때 속도 감쇠
    public float minSlideSpeed = 0.5f;  // 너무 느린 미끄러짐을 방지하는 최소 속도
    private Animator animator;
    private int projectilesOnField = 0;
    private int projectilesFiredThisTurn = 0;
    private float chaseStartTime;
    private bool isAttacking = false;
    public float chaseDuration = 3f; // 추적 시간
    private void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();
        seeker = GetComponent<Seeker>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        // AIPath의 기본 움직임 적용
        Vector2 movement = aiPath.desiredVelocity;

        // NPC가 벽에 부딪혔을 때 미끄러지기
        if (rb.velocity.magnitude > minSlideSpeed)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, movement, slideFriction * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.zero;  // 속도가 너무 낮으면 멈추기
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTurnComplete)
        {
            Debug.Log($"현재 턴이 아님: {gameObject.name}");
            return;
        }

        // 다른 NPC와 충돌했을 때
        if (collision.collider.CompareTag("NPC"))
        {
            Debug.Log("NPC와 충돌!");

            // 충돌 방향 계산
            Vector2 collisionDirection = (collision.transform.position - transform.position).normalized;

            // 밀어낼 각도 설정 (예: 30도)
            float angle = 30f;

            // 왼쪽 또는 오른쪽으로 회전
            float pushAngle = Random.value > 0.5f ? angle : -angle;

            // 밀어내는 방향 계산
            Vector2 pushDirection = RotateVector(collisionDirection, pushAngle);

            // 충돌한 상대 NPC의 aipath 가져오기
            AIPath otherAIPath = collision.collider.GetComponent<AIPath>();

            if (otherAIPath != null)
            {
                StartCoroutine(PauseAndPushOtherNPC(otherAIPath, pushDirection));
            }
        }
    }

    // 벡터 회전 함수
    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float x = vector.x * cos - vector.y * sin;
        float y = vector.x * sin + vector.y * cos;

        return new Vector2(x, y).normalized;
    }

    private IEnumerator PauseAndPushOtherNPC(AIPath otherAIPath, Vector2 pushDirection)
    {
        // 현재 AIPath의 원래 목적지 저장
        

        // 밀려날 목표 위치 계산 (Z축 0으로 설정하여 2D 환경에 맞춤)
        float pushDistance = 1.5f; // 밀려날 거리
        Vector2 pushOffset = pushDirection * pushDistance;
        Vector3 pushDestination = new Vector3(
            otherAIPath.transform.position.x + pushOffset.x,
            otherAIPath.transform.position.y + pushOffset.y,
            0f // Z축 값은 0으로 설정하여 2D 평면 유지
        );

        // 자신 NPC의 AIPath 가져와 이동 멈춤
        AIPath selfAIPath = GetComponent<AIPath>();
        selfAIPath.canMove = false;

        // 상대 NPC의 목표 위치 설정
        otherAIPath.destination = pushDestination;
        otherAIPath.canMove = true; // 상대 NPC 이동 활성화
        otherAIPath.endReachedDistance = 0.1f; // 목적지 도달 거리 조정

        // 밀려나는 시간 동안 대기
        float pushDuration = 0.5f;
        yield return new WaitForSeconds(pushDuration);

       

        // 자신 NPC 3초 대기
        float pauseDuration = 1.5f;
        yield return new WaitForSeconds(pauseDuration);
        otherAIPath.endReachedDistance = 5.0f; // 목적지 도달 거리 조정
        // 자신 NPC 이동 재개
        selfAIPath.canMove = true;
    }
    

    public void StartTurn()
    {
        isTurnComplete = false;
        animator.SetBool("IsWalking", false);
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        if (directionToPlayer.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionToPlayer.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        MoveTowardsPlayer(directionToPlayer);
        chaseStartTime = Time.time; // 추적 시작 시간 기록 // 5초 후 턴 종료
    }



    private void MoveTowardsPlayer(Vector2 x)
    {
        Vector2 directionToPlayer = x;
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, coverLayer);

        if (hit.collider != null)
        {
            Vector2 avoidancePosition = hit.point - directionToPlayer * 0.5f;
            MoveToPosition(avoidancePosition);
        }
        else
        {
            MoveToPosition(player.transform.position);
        }
    }

    private void MoveToPosition(Vector2 position)
    {
        aiPath.destination = position;
        aiPath.canMove = true;
        isMoving = true;
    }

    private void Update()
    {

        if (!isTurnComplete)
        {
            // 플레이어를 추적
            aiPath.destination = player.transform.position;
            bool isMoving = aiPath.remainingDistance > aiPath.endReachedDistance;
            animator.SetBool("IsWalking", isMoving);

            // 3초가 지나면 멈추고 공격
            if (!isAttacking && Time.time - chaseStartTime >= chaseDuration)
            {
                StartCoroutine(AttackAndEndTurn()); // 코루틴으로 실행
                isAttacking = true; // 공격 중 상태로 설정

            }
            

        }
        // NPC의 x축 움직임에 따라 좌우 반전 설정
        if (aiPath.desiredVelocity.x < -0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (aiPath.desiredVelocity.x > 0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }



    public void EndTurn()
    {
        isTurnComplete = true;
        aiPath.destination = transform.position; // 멈추기 위해 현재 위치를 목적지로 설정
        animator.SetBool("IsWalking", false);
        TurnManager.Instance.NextTurn();
    }
    private IEnumerator AttackAndEndTurn()
    {
        // 공격 애니메이션 실행 및 공격 로직
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

        Attack(directionToPlayer);
        Debug.Log("공격");

        // 공격 후 대기 시간 추가
        yield return new WaitForSeconds(2.0f); // 공격 후 2초 대기

        Debug.Log("턴넘기기");

        EndTurn();
        isAttacking = false;
    }


    public void Attack(Vector2 direction)
    {
        animator.SetTrigger("Attack");

        // 발사체 생성 위치를 NPC 위치보다 약간 아래로 설정
        Vector3 projectileSpawnPosition = transform.position + Vector3.left * 0.1f + Vector3.down * 0.3f; //  (값 조정 가능)
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPosition, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;

        // 자신의 콜라이더와 발사체의 충돌을 무시
        Collider2D npcCollider = GetComponent<Collider2D>();
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        if (projectileCollider != null && npcCollider != null)
        {
            Physics2D.IgnoreCollision(projectileCollider, npcCollider);
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

    // ICombatant 인터페이스 구현
    public int MaxActionPoints => 0; // 더 이상 사용하지 않음

    public int CurrentActionPoints
    {
        get => 0; // 더 이상 사용하지 않음
        set { }
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
