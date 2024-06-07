using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject projectilePrefab; // ����ü Prefab
    public int maxProjectiles = 3; // �ִ� �߻�ü ��
    public int projectileDamage = 10; // �߻�ü ������
    [HideInInspector]
    public int currentProjectiles = 0; // ���� �߻�ü ��
    private LineRenderer lineRenderer;
    private Vector2 aimDirection;

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
            if (Input.GetMouseButton(0)) // ���콺 ��Ŭ�� ����
            {
                ShowAim();
            }

            if (Input.GetMouseButtonUp(0)) // ���콺 ��Ŭ�� ����
            {
                if (currentProjectiles < maxProjectiles)
                {
                    ShootProjectile();
                }
                else
                {
                    lineRenderer.positionCount = 0; // ��� �����
                }
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

    void ShootProjectile()
    {
        lineRenderer.positionCount = 0; // ��� �����

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb == null)
        {
            projectileRb = projectile.AddComponent<Rigidbody2D>();
        }
        projectileRb.isKinematic = true; // �߻�ü�� Kinematic���� ����
        projectileRb.velocity = aimDirection * 5f; // �߻�ü �ӵ� ���� (������ ������ ����)

        // �߻�ü�� �÷��̾��� �浹 ����
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // �߻�ü�� �浹 �ڵ鷯 �߰� �� ������ ����
        ProjectileCollisionHandler collisionHandler = projectile.AddComponent<ProjectileCollisionHandler>();
        collisionHandler.damage = projectileDamage;
        collisionHandler.projectileOwner = this; // �߻�ü ������ ����

        currentProjectiles++; // ���� �߻�ü �� ����
        StartCoroutine(DestroyProjectileAfterTime(projectile, 5f)); // 5�� �Ŀ� �߻�ü ����
    }

    IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        if (projectile != null)
        {
            Destroy(projectile);
            currentProjectiles--; // �߻�ü �� ����
        }
    }

    public void NotifyProjectileDestroyed()
    {
        currentProjectiles--;
    }
}
