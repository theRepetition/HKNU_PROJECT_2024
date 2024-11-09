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
            turnUI = FindObjectOfType<TurnUI>(); // TurnUI ������Ʈ ã��
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

        // �������� �����ϰ� ����
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
            turnUI.ClearTurnText(); // ��� ��ȯ �� TurnUI ������Ʈ
        }
    }

    public GameMode GetCurrentMode()
    {
        return currentMode;
    }
}
