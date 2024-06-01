using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damagePerSecond = 1; // 초당 데미지량
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            StartCoroutine(ApplyDamage(other.GetComponent<Health>()));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")|| other.CompareTag("NPC"))
        {
            StopAllCoroutines(); // 플레이어가 영역을 떠나면 데미지 중지
        }
    }

    private IEnumerator ApplyDamage(Health playerHealth)
    {
        while (true)
        {
            playerHealth.TakeDamage(damagePerSecond);
            yield return new WaitForSeconds(1f); // 1초마다 데미지 적용
        }
    }
}
