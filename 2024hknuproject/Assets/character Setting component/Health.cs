using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public SpriteRenderer spriteRenderer;
    public float blinkDuration = 0.1f;  // 깜빡임 간격
    public int blinkCount = 3;
    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            StartCoroutine(BlinkCoroutine());
        }
    }
    private IEnumerator BlinkCoroutine()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);  // 투명하게
            yield return new WaitForSeconds(blinkDuration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);  // 다시 보이게
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log(gameObject.name + " healed " + amount + ". Current health: " + currentHealth);
    }

    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " 사망.");
        
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
