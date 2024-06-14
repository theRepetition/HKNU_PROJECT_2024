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
    private bool isTurnComplete; // �� ���� �߰��մϴ�.
    private Rigidbody2D rb;
    public LayerMask coverLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator ExecuteCombatAI(int actionPoints)
    {
        // ������ ã�� �̵�
        MoveToCover(actionPoints);
        yield return new WaitForSeconds(1.0f);

        // �÷��̾� ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Attack(direction);
            yield return new WaitForSeconds(1.0f);
        }

        EndTurn(); // ���� �� �� ����
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
        StartCoroutine(ExecuteCombatAI(currentActionPoints));
    }

    public void EndTurn()
    {
        isTurnComplete = true; // �� �Ϸ� ����
        TurnManager.Instance.NextTurn(); // �� ���� �� ���� ������ ��ȯ
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
