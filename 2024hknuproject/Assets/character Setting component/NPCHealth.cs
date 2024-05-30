using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : Health
{
    protected override void Die()
    {
        base.Die();
        // NPC가 죽었을 때의 동작 추가
        Debug.Log("NPC has died. Recording death for later use.");
        // 예: 기록 저장
        // NPCManager.Instance.RecordNPCDeath(this);
        Destroy(gameObject);
    }
}
