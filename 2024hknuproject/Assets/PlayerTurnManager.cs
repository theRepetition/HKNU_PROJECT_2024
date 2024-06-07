using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnManager : MonoBehaviour, ITurnTaker
{
    public int maxActionPoints = 10; // 최대 행동력
    private int currentActionPoints;
    private bool isTurnComplete = false;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        TurnManager.Instance.RegisterTurnTaker(this);
    }

    void Update()
    {
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && TurnManager.Instance.CurrentTurnTaker == this)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isTurnComplete && playerCombat.currentProjectiles == 0)
            {
                EndTurn();
            }
        }
    }

    public void StartTurn()
    {
        currentActionPoints = maxActionPoints; // 턴이 시작될 때 행동력 초기화
        isTurnComplete = false; // 턴 시작 시 초기화
        playerMovement.EnableMovement();
        Debug.Log("내 턴 시작");
    }

    public void EndTurn()
    {
        playerMovement.DisableMovement();
        isTurnComplete = true; // 턴 완료 설정
        Debug.Log("턴 종료");
        TurnManager.Instance.NextTurn(); // 턴 종료 후 다음 턴으로 전환
    }

    public bool IsTurnComplete => isTurnComplete; // 턴 완료 여부

    public string Name => gameObject.name; // 이름 반환
}
