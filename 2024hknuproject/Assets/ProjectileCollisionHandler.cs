using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    public int damage = 10;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"발사체 충돌: {collision.gameObject.name}");

        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            Debug.Log($"대상 체력 감소: {targetHealth.GetCurrentHealth()}");
        }

        // 충돌 시 발사체를 즉시 제거
        Destroy(gameObject);
    }
}
