using System.Collections;
using UnityEngine;

public class NPCCombatAI : MonoBehaviour, ICombatant
{
    public GameObject projectilePrefab; // ����ü Prefab
    public float projectileSpeed = 5f; // ����ü �ӵ�
    public int projectileDamage = 10; // ����ü ������
    public int maxProjectilesPerTurn = 3; // �ϴ� �ִ� �߻�ü ��
    private int projectilesFiredThisTurn = 0; // ���� �Ͽ��� �߻��� �߻�ü ��
    private int projectilesOnField = 0; // �ʵ忡 �����ϴ� �߻�ü ��
    public int maxActionPoints = 5; // �ִ� �ൿ��
    private int currentActionPoints;
    private bool isTurnComplete; // �� �Ϸ� Ȯ��
    private Rigidbody2D rb;
    public LayerMask coverLayer;
    public float weaponRange = 10f; // ������ �����Ÿ�
    public float moveSpeed = 2f; // �̵� �ӵ�

    private Vector2 targetPosition;
    private bool isMoving;
    private float timeAtSamePosition; // ���� ��ġ�� �ӹ� �ð�
    private Vector2 lastPosition; // ������ ��ġ

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
                    EndTurn(); // 3�� ���� �������� ������ �� ����
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

            // �÷��̾���� �Ÿ��� ����Ͽ� ������ ��ġ�� �̵�
            if (distanceToPlayer > weaponRange)
            {
                MoveToPosition((Vector2)player.transform.position); // player.transform.position�� Vector2�� ��ȯ
            }

            yield return new WaitForSeconds(1.0f);

            // �÷��̾� ����
            Attack(directionToPlayer);
            yield return new WaitForSeconds(1.0f);
        }

        // �÷��̾�� NPC ������ ���� Ž�� �� �̵�
        FindBestCoverAndMove();

        yield return new WaitForSeconds(3.0f); // NPC�� 3�� ���� �������� ������ �� ����
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

            // �÷��̾�� ������ NPC-����-�÷��̾�� �������� �ִ��� Ȯ��
            float dotProduct = Vector2.Dot(directionToCover, directionToPlayerFromCover);

            // �������� �������� ������ ����, NPC�� ���� ������ �Ÿ��� ���� ����
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
        Vector2 optimalPosition = coverPosition - directionToPlayer * 1.0f; // ���� �� 1.0f �Ÿ��� �̵�
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
        projectileRb.isKinematic = true; // �߻�ü�� Kinematic���� ����
        projectileRb.velocity = direction * projectileSpeed; // �߻�ü �ӵ� ����

        // �߻�ü�� NPC�� �浹 ����
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // �߻�ü�� �浹 �ڵ鷯 �߰� �� ������ ����
        ProjectileCollisionHandler collisionHandler = projectile.AddComponent<ProjectileCollisionHandler>();
        collisionHandler.damage = projectileDamage;
        collisionHandler.projectileOwner = this;

        projectilesFiredThisTurn++; // ���� �Ͽ��� �߻��� �߻�ü �� ����
        projectilesOnField++; // �ʵ忡 �����ϴ� �߻�ü �� ����
        StartCoroutine(DestroyProjectileAfterTime(projectile, 5f)); // 5�� �Ŀ� �߻�ü ����
    }

    IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        if (projectile != null)
        {
            Destroy(projectile);
            projectilesOnField--; // �ʵ忡 �����ϴ� �߻�ü �� ����
        }
    }

    public void NotifyProjectileDestroyed()
    {
        projectilesOnField--; // �ʵ忡 �����ϴ� �߻�ü �� ����
    }

    public void ResetProjectilesFired()
    {
        projectilesFiredThisTurn = 0; // �߻�ü �� �ʱ�ȭ
    }

    public void StartTurn()
    {
        currentActionPoints = maxActionPoints; // ���� ���۵� �� �ൿ�� �ʱ�ȭ
        isTurnComplete = false; // �� ���� �� �ʱ�ȭ
        ResetProjectilesFired(); // �߻�ü �� �ʱ�ȭ
        timeAtSamePosition = 0f; // �ð��� �ʱ�ȭ
        lastPosition = rb.position; // ���� ��ġ�� ����
        StartCoroutine(ExecuteCombatAI(currentActionPoints));
    }

    public void EndTurn()
    {
        isTurnComplete = true; // �� �Ϸ� ����
        TurnManager.Instance.NextTurn(); // �� ���� �� ���� ������ ��ȯ
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
