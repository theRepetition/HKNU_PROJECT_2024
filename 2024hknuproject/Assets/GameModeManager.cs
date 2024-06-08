using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    public enum GameMode { RealTime, TurnBased }
    public GameMode currentMode = GameMode.RealTime;

    private TurnUI turnUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            turnUI = FindObjectOfType<TurnUI>(); // TurnUI 컴포넌트 찾기
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchToTurnBasedMode()
    {
        currentMode = GameMode.TurnBased;
        Debug.Log("Switched to Turn-Based Mode");

        // 선공권을 랜덤하게 설정
        ITurnTaker firstTurnTaker = TurnManager.Instance.GetRandomTurnTaker();
        TurnManager.Instance.SetFirstTurnTaker(firstTurnTaker);

        TurnManager.Instance.StartTurnBasedMode();
    }

    public void SwitchToRealTimeMode()
    {
        currentMode = GameMode.RealTime;
        Debug.Log("Switched to Real-Time Mode");
        if (turnUI != null)
        {
            turnUI.ClearTurnText(); // 모드 전환 시 TurnUI 업데이트
        }
    }

    public GameMode GetCurrentMode()
    {
        return currentMode;
    }
}
