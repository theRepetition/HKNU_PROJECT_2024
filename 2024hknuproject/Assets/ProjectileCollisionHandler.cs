using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    public int damage = 10;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"�߻�ü �浹: {collision.gameObject.name}");

        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            Debug.Log($"��� ü�� ����: {targetHealth.GetCurrentHealth()}");
        }

        // �浹 �� �߻�ü�� ��� ����
        Destroy(gameObject);
    }
}
