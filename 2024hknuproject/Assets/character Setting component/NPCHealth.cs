using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : Health
{

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.1f); // 약간의 지연
        TurnManager.Instance.CheckForRealTimeMode();
        Destroy(gameObject); // 검사가 끝난 후 오브젝트를 제거
    }
}
