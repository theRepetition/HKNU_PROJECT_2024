using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTurnBasedMode()
    {
        StartCoroutine(TurnRoutine());
    }

    private IEnumerator TurnRoutine()
    {
        while (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            Debug.Log("Player's Turn");
            PlayerMovement2D playerMovement = FindObjectOfType<PlayerMovement2D>();
            if (playerMovement != null)
            {
                playerMovement.EnableMovement();
            }

            yield return PlayerTurn();

            if (playerMovement != null)
            {
                playerMovement.DisableMovement();
            }

            Debug.Log("NPC's Turn");
            yield return NPCTurn();
        }
    }

    private IEnumerator PlayerTurn()
    {
        bool playerTurnEnded = false;

        // 플레이어가 스페이스바를 눌러 턴을 끝낼 때까지 대기
        while (!playerTurnEnded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerTurnEnded = true;
                Debug.Log("Player ends turn");
            }
            yield return null;
        }
    }

    private IEnumerator NPCTurn()
    {
        // NPC 턴을 3초간 유지
        yield return new WaitForSeconds(3.0f);
        Debug.Log("NPC ends turn");
    }
}
