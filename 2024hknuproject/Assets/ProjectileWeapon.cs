using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour, IWeapon
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public int damage = 10;

    public void Attack(Vector2 targetPosition)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb == null)
        {
            projectileRb = projectile.AddComponent<Rigidbody2D>();
        }
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        projectileRb.velocity = direction * projectileSpeed;

        // Add collision handler to projectile
        ProjectileCollisionHandler collisionHandler = projectile.GetComponent<ProjectileCollisionHandler>();
        if (collisionHandler == null)
        {
            collisionHandler = projectile.AddComponent<ProjectileCollisionHandler>();
        }
        collisionHandler.damage = damage;
        collisionHandler.projectileOwner = null; // NPC의 소유자로 설정하지 않음
    }
}