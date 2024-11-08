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
        // 벽에 부딪혔을 때 미끄러지기 시작
        if (collision.collider.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;  // 벽의 법선 벡터
            Vector2 slideDirection = Vector2.Perpendicular(normal);  // 벽에 수직인 방향으로 미끄러짐
            rb.velocity = slideDirection * rb.velocity.magnitude * slideFriction;
        }
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
