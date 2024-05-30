using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    protected override void Die()
    {
        base.Die();
        // 플레이어가 죽었을 때의 동작 추가
        Debug.Log("Player has died. Respawn or end game.");
        // 예: 게임 종료 또는 리스폰 처리
        // GameManager.Instance.EndGame();
        // or
        // GameManager.Instance.RespawnPlayer();
    }
}
