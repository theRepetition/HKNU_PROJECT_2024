using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    protected override void Die()
    {
        base.Die();
        // �÷��̾ �׾��� ���� ���� �߰�
        Debug.Log("Player has died. Respawn or end game.");
        // ��: ���� ���� �Ǵ� ������ ó��
        // GameManager.Instance.EndGame();
        // or
        // GameManager.Instance.RespawnPlayer();
    }
}
