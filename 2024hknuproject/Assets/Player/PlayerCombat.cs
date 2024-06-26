using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ICombatant
{
    public GameObject projectilePrefab; // ����ü Prefab
    public float projectileSpeed = 5f; // ����ü �ӵ�
    public int projectileDamage = 10; // ����ü ������
    public int maxProjectilesPerTurn = 3; // �ϴ� �ִ� �߻�ü ��
    private int projectilesFiredThisTurn = 0; // ���� �Ͽ��� �߻��� �߻�ü ��
    private int projectilesOnField = 0; // �ʵ忡 �����ϴ� �߻�ü ��
    public int maxActionPoints = 10; // �ִ� �ൿ��
    private int currentActionPoints;
    private LineRenderer lineRenderer;
    private Vector2 aimDirection;
    private bool isTurnComplete = false;

    private HUDManager hudManager; // HUDManager ����

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // �ʱ⿡�� ��θ� ǥ������ ����

        // LineRenderer�� Ÿ�� ���� ���̵��� Z�� ����
        lineRenderer.sortingOrder = 1;

        hudManager = FindObjectOfType<HUDManager>(); // HUDManager ã��
        if (hudManager != null)
        {
            hudManager.UpdateBulletSlots(maxProjectilesPerTurn - projectilesFiredThisTurn); // HUD �ʱ�ȭ
        }
    }

    void Update()
    {
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && TurnManager.Instance.CurrentTurnTaker == GetComponent<PlayerTurnManager>())
        {
            if (Input.GetMouseButton(0) && projectilesFiredThisTurn < maxProjectilesPerTurn) // ���콺 ��Ŭ�� ����
            {
                ShowAim();
            }

            if (Input.GetMouseButtonUp(0) && projectilesFiredThisTurn < maxProjectilesPerTurn) // ���콺 ��Ŭ�� ����
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
                lineRenderer.positionCount = 0; // ��� �����
            }

            if (Input.GetKeyDown(KeyCode.R)) // ������ Ű (��: R Ű)
            {
                Reload();
            }
        }
    }

    void ShowAim()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimDirection = (mousePosition - transform.position).normalized;

        // ��� ǥ��
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + (Vector3)(aimDirection * 10f)); // ������ ���� ����

        // LineRenderer�� Ÿ�� ���� ���̵��� Z�� ����
        lineRenderer.SetPosition(0, new Vector3(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(0).y, -1));
        lineRenderer.SetPosition(1, new Vector3(lineRenderer.GetPosition(1).x, lineRenderer.GetPosition(1).y, -1));
    }

    public void Attack(Vector2 direction)
    {
        if (projectilesFiredThisTurn >= maxProjectilesPerTurn) // źȯ�� �� �Ҹ�Ǹ� ���� �Ұ�
            return;

        lineRenderer.positionCount = 0; // ��� �����

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb == null)
        {
            projectileRb = projectile.AddComponent<Rigidbody2D>();
        }
        projectileRb.isKinematic = true; // �߻�ü�� Kinematic���� ����
        projectileRb.velocity = direction * projectileSpeed; // �߻�ü �ӵ� ����

        // �߻�ü�� �÷��̾��� �浹 ����
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // �߻�ü�� �浹 �ڵ鷯 �߰� �� ������ ����
        ProjectileCollisionHandler collisionHandler = projectile.AddComponent<ProjectileCollisionHandler>();
        collisionHandler.damage = projectileDamage;
        collisionHandler.projectileOwner = this;

        projectilesFiredThisTurn++; // ���� �Ͽ��� �߻��� �߻�ü �� ����
        projectilesOnField++; // �ʵ忡 �����ϴ� �߻�ü �� ����

        hudManager.UpdateBulletSlots(maxProjectilesPerTurn - projectilesFiredThisTurn); // HUD ������Ʈ

        StartCoroutine(DestroyProjectileAfterTime(projectile, 5f)); // 5�� �Ŀ� �߻�ü ����
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
        projectilesOnField--; // �ʵ忡 �����ϴ� �߻�ü �� ����
    }

    public void ResetProjectilesFired()
    {
        projectilesFiredThisTurn = 0; // �߻�ü �� �ʱ�ȭ
        hudManager.UpdateBulletSlots(maxProjectilesPerTurn - projectilesFiredThisTurn); // HUD ������Ʈ
    }

    public void Reload()
    {
        projectilesFiredThisTurn = 0; // �߻�ü �� �ʱ�ȭ
        Debug.Log("������ �Ϸ�");

        hudManager.UpdateBulletSlots(maxProjectilesPerTurn); // HUD ������Ʈ

        // ���� ����� �� �������ϸ� �� ����
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            EndTurn();
        }
    }

    public void StartTurn()
    {
        isTurnComplete = false; // �� ���� �� �ʱ�ȭ
        ResetProjectilesFired(); // �߻�ü �� �ʱ�ȭ
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
