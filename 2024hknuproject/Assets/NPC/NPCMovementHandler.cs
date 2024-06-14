using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovementHandler : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator MoveRandomly(int actionPoints)
    {
        float elapsed = 0f;
        float turnDuration = 3.0f;

        while (elapsed < turnDuration)
        {
            if (actionPoints > 0)
            {
                Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                MoveCharacter(direction);
                yield return new WaitForSeconds(1.0f);
                elapsed += 1.0f;
            }
            else
            {
                yield return null;
            }
        }
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
