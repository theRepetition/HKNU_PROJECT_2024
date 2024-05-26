using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    public enum GameMode { RealTime, TurnBased }
    public GameMode currentMode = GameMode.RealTime;

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

    public void SwitchToTurnBasedMode()
    {
        currentMode = GameMode.TurnBased;
        Debug.Log("Switched to Turn-Based Mode");
        TurnManager.Instance.StartTurnBasedMode();
    }

    public void SwitchToRealTimeMode()
    {
        currentMode = GameMode.RealTime;
        Debug.Log("Switched to Real-Time Mode");
    }
}
