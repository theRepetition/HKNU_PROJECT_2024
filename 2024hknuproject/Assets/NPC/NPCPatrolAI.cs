using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPatrolAI : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Patrol()
    {
        // ������ ���¿��� NPC�� �̵��ϴ� �ڵ�
        Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        MoveCharacter(direction);
    }

    private void MoveCharacter(Vector2 direction)
    {
        Vector2 newPosition = rb.position + direction * 3f * Time.fixedDeltaTime;
        float distance = Vector2.Distance(rb.position, newPosition);

        if (distance > 0)
        {
            rb.MovePosition(newPosition);
        }
    }
}
