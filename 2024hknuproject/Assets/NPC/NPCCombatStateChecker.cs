using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCombatStateChecker : MonoBehaviour
{
    public LayerMask playerLayer;

    public bool CheckForPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 5.0f, playerLayer);
        return hit.collider != null && hit.collider.CompareTag("Player");
    }
}
