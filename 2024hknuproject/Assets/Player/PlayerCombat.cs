using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ICombatant
{
    public GameObject projectilePrefab; // ����ü Prefab
    public float projectileSpeed = 5f; // ����ü �ӵ�
    public int maxProjectilesPerTurn = 6; // �ϴ� �ִ� �߻�ü ��
    private int projectilesFiredThisTurn = 0; // ���� �Ͽ��� �߻��� �߻�ü ��
    private int projectilesOnField = 0; // �ʵ忡 �����ϴ� �߻�ü ��
    public int maxActionPoints = 10; // �ִ� �ൿ��
    private int currentActionPoints;
    private LineRenderer lineRenderer;
    private Vector2 aimDirection;
    private bool isTurnComplete = false;
    private List<Ammo> currentLoadedAmmo = new List<Ammo>(); // ���� ������ ź�� ����Ʈ
    private PlayerTurnManager playerturnmanager; // PlayerTurnManager ��ũ��Ʈ ����


    private Ammo currentAmmo; // ���� ������ ź��

    public int ProjectileDamage => currentLoadedAmmo.Count > 0 ? currentLoadedAmmo[0].damage : 0;
    // ź���� ���ط��� ��ȯ

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0; // �ʱ⿡�� ��θ� ǥ������ ����

        // LineRenderer�� Ÿ�� ���� ���̵��� Z�� ����
        lineRenderer.sortingOrder = 1;
        



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
                playerturnmanager.EndTurn();
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
    public void LogLoadedAmmo()
    {
        Debug.Log("Currently Loaded Ammo:");
        for (int i = 0; i < currentLoadedAmmo.Count; i++)
        {
            Debug.Log($"Slot {i + 1}: {currentLoadedAmmo[i].itemName}, Quantity: {currentLoadedAmmo[i].quantity}");
        }
    }

    public void Attack(Vector2 direction)
    {
        if (projectilesFiredThisTurn >= maxProjectilesPerTurn || currentLoadedAmmo.Count == 0) // źȯ�� �� �Ҹ�Ǹ� ���� �Ұ�
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
        collisionHandler.damage = currentLoadedAmmo[0].damage; // ź���� ���ط� ���
        collisionHandler.projectileOwner = this;

        projectilesFiredThisTurn++; // ���� �Ͽ��� �߻��� �߻�ü �� ����
        projectilesOnField++; // �ʵ忡 �����ϴ� �߻�ü �� ����

        currentLoadedAmmo.RemoveAt(0); // ù ��° źȯ�� ����

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

    }

    public void LoadAmmo(Ammo ammo)
    {
        Debug.LogError("123");
        if (currentLoadedAmmo.Count < maxProjectilesPerTurn)
        {
            Debug.Log($"Loading ammo: {ammo.itemName} into chamber.");
            currentLoadedAmmo.Add(ammo);
            Debug.Log($"Current loaded ammo count: {currentLoadedAmmo.Count}");

            
        }
        else
        {
            Debug.LogError("źâ ����");
        }
    }




    public void StartTurn()
    {
        isTurnComplete = false; // �� ���� �� �ʱ�ȭ
        ResetProjectilesFired(); // �߻�ü �� �ʱ�ȭ
    }

    public void EndTurn()
    {

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

    public int ProjectilesOnField
    {
        get => projectilesOnField;
        set => projectilesOnField = value;
    }

    public void SetLoadedAmmo(List<Ammo> loadedAmmo)
    {
        Debug.Log("SetLoadedAmmo called.");
        Debug.Log($"Loaded ammo count: {loadedAmmo.Count}");
        
        currentLoadedAmmo = new List<Ammo>(loadedAmmo); // ������ ź�� ����
    }
}
