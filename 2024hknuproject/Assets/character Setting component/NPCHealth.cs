using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : Health
{
    protected override void Die()
    {
        base.Die();
        // NPC�� �׾��� ���� ���� �߰�
        Debug.Log("NPC has died. Recording death for later use.");
        // ��: ��� ����
        // NPCManager.Instance.RecordNPCDeath(this);
        
        StartCoroutine(HandleDeath());
        TurnManager.Instance.NextTurn();
    }
    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.1f); // �ణ�� ����
        TurnManager.Instance.CheckForRealTimeMode();
        Destroy(gameObject); // �˻簡 ���� �� ������Ʈ�� ����
    }
}
