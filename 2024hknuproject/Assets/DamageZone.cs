using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damagePerSecond = 1; // �ʴ� ��������
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
            StopAllCoroutines(); // �÷��̾ ������ ������ ������ ����
        }
    }

    private IEnumerator ApplyDamage(Health playerHealth)
    {
        while (true)
        {
            playerHealth.TakeDamage(damagePerSecond);
            yield return new WaitForSeconds(1f); // 1�ʸ��� ������ ����
        }
    }
}
