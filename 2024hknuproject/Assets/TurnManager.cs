using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private List<ITurnTaker> turnTakers = new List<ITurnTaker>();
    private int currentTurnIndex;
    public ITurnTaker CurrentTurnTaker { get; private set; }

    public List<ITurnTaker> TurnTakers => turnTakers; // TurnTakers 리스트에 대한 접근자

    public string npcLayerName = "npcLayer"; // NPC 레이어 이름
    private int npcLayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            npcLayer = LayerMask.NameToLayer(npcLayerName); // NPC 레이어 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private IEnumerator LogTurnTakerLayers()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            LogCurrentTurnTakers();
        }
    }

    private void LogCurrentTurnTakers()
    {
        Debug.Log("Current TurnTakers:");
        if (turnTakers.Count == 0)
        {
            Debug.Log("No TurnTakers registered.");
        }
        foreach (var turnTaker in turnTakers)
        {
            var monoBehaviour = turnTaker as MonoBehaviour;
            if (monoBehaviour != null)
            {
                Debug.Log($"TurnTaker: {LayerMask.LayerToName(monoBehaviour.gameObject.layer)}");
            }
        }
    }

    public void RegisterTurnTaker(ITurnTaker turnTaker)
    {
        turnTakers.Add(turnTaker);
        Debug.Log($"Registered TurnTaker: {LayerMask.LayerToName((turnTaker as MonoBehaviour).gameObject.layer)}");
    }

    public void UnregisterTurnTaker(ITurnTaker turnTaker)
    {
        Debug.Log($"UnregisterTurnTaker called for: {LayerMask.LayerToName((turnTaker as MonoBehaviour).gameObject.layer)}");

        if (turnTakers.Contains(turnTaker))
        {
            int indexToRemove = turnTakers.IndexOf(turnTaker);
            turnTakers.Remove(turnTaker);
            Debug.Log($"Unregistered TurnTaker: {LayerMask.LayerToName((turnTaker as MonoBehaviour).gameObject.layer)}");

            // currentTurnIndex를 조정함
            if (indexToRemove <= currentTurnIndex && currentTurnIndex > 0)
            {
                currentTurnIndex--;
            }
        }

        Debug.Log("UnregisterTurnTaker finished.");
        // NPC가 없으면 실시간 모드로 전환함
        CheckForRealTimeMode();
    }

    public void CheckForRealTimeMode()
    {
        Debug.Log("모드 확인");

        bool hasNPC = false;
        foreach (var turnTaker in turnTakers)
        {
            var monoBehaviour = turnTaker as MonoBehaviour;
            if (monoBehaviour != null && monoBehaviour.gameObject.layer == npcLayer)
            {
                hasNPC = true;
                break;
            }
        }

        if (!hasNPC)
        {
            Debug.Log("턴제 종료");
            GameModeManager.Instance.SwitchToRealTimeMode();
            StopAllCoroutines(); // 모든 코루틴을 중지하여 턴 루틴을 종료함
        }
        else
        {
            Debug.Log("턴제 진행");
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
            yield return StartCoroutine(TakeTurns());
        }
    }

    private IEnumerator TakeTurns()
    {
        while (true)
        {
            if (turnTakers.Count == 0)
            {
                GameModeManager.Instance.SwitchToRealTimeMode();
                yield break;
            }

            CurrentTurnTaker = turnTakers[currentTurnIndex];

            // CurrentTurnTaker가 유효한지 확인함
            if (CurrentTurnTaker == null || CurrentTurnTaker as MonoBehaviour == null)
            {
                turnTakers.RemoveAt(currentTurnIndex);
                if (currentTurnIndex >= turnTakers.Count)
                {
                    currentTurnIndex = 0;
                }

                // NPC가 없으면 실시간 모드로 전환함
                CheckForRealTimeMode();
                continue;
            }

            CurrentTurnTaker.StartTurn();

            while (!CurrentTurnTaker.IsTurnComplete)
            {
                yield return null;
            }

            CurrentTurnTaker.EndTurn();

            // NPC가 없으면 실시간 모드로 전환함
            CheckForRealTimeMode();

            currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;

            CheckForRealTimeMode();
        }
    }

    public void NextTurn()
    {
        currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;

        // 현재 턴 테이커가 존재하지 않으면 NPC 검사 수행
        if (turnTakers.Count > 0)
        {
            CurrentTurnTaker = turnTakers[currentTurnIndex];
            if (CurrentTurnTaker != null)
            {
                CurrentTurnTaker.StartTurn();
            }
        }

        // NPC가 없는지 확인
        CheckForRealTimeMode();
    }
}
