using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : Health
{
    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.1f);
        TurnManager.Instance.CheckForRealTimeMode();
        Destroy(gameObject);
    }

    protected override void Die()
    {
        base.Die();
        StartCoroutine(HandleDeath());
    }
}
