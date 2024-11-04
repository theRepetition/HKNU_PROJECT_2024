using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    public int damage = 10;
    public ICombatant projectileOwner;

    void OnCollisionEnter2D(Collision2D collision)
    {
        

        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            
        }

        
        Destroy(gameObject);
        if (projectileOwner != null)
        {
            projectileOwner.NotifyProjectileDestroyed();
        }
    }
}
