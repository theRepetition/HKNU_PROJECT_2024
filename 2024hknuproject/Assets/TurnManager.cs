using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private List<ITurnTaker> turnTakers = new List<ITurnTaker>(); // 턴 참여자 목록
    private int currentTurnIndex; // 현재 턴 인덱스
    public ITurnTaker CurrentTurnTaker { get; private set; } // 현재 턴을 가진 참여자

    public List<ITurnTaker> TurnTakers => turnTakers; // 턴 참여자 목록 반환

    public string npcLayerName = "npcLayer"; // NPC 레이어 이름
    private int npcLayer;
    public float maxDistanceFromPlayer = 15f; // 플레이어와 NPC 간의 최대 거리

    private void Awake()
    {
        if (turnTakers.Count == 0)
        {
            // 실시간 모드로 전환
            GameModeManager.Instance?.SwitchToRealTimeMode();
            Debug.Log("No TurnTakers found. Switched to real-time mode.");
        }
        // 싱글톤 패턴 구현
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

    // 턴 참여자를 등록하는 함수
    public void RegisterTurnTaker(ITurnTaker turnTaker)
    {
        // 중복 등록 방지: 리스트에 이미 있는지 확인
        if (!turnTakers.Contains(turnTaker))
        {
            turnTakers.Add(turnTaker);
        }
        else
        {
            Debug.LogWarning($"TurnTaker '{(turnTaker as MonoBehaviour)?.gameObject.name}' is already registered.");
        }
    }

    // NPC가 플레이어 근처에 있는지 확인하는 함수
    private bool IsNpcInRangeOfPlayer(ITurnTaker npc, float range)
    {
        var npcObject = (npc as MonoBehaviour)?.gameObject;
        var playerObject = GameObject.FindGameObjectWithTag("Player");

        if (npcObject == null || playerObject == null)
            return false;

        return Vector2.Distance(npcObject.transform.position, playerObject.transform.position) <= range;
    }

    // 턴 참여자를 해제하는 함수
    public void UnregisterTurnTaker(ITurnTaker turnTaker)
    {
        if (turnTakers.Contains(turnTaker))
        {
            int indexToRemove = turnTakers.IndexOf(turnTaker);
            turnTakers.Remove(turnTaker);

            // 현재 턴 인덱스를 조정
            if (indexToRemove <= currentTurnIndex && currentTurnIndex > 0)
            {
                currentTurnIndex--;
            }
        }
    }

    // NPC가 없으면 실시간 모드로 전환하는 함수
    public void CheckForRealTimeMode()
    {
        bool hasNPC = false;
        foreach (var turnTaker in turnTakers)
        {
            var monoBehaviour = turnTaker as MonoBehaviour;
            if (monoBehaviour != null && monoBehaviour.CompareTag("NPC"))
            {
                hasNPC = true;
                break;
            }
        }

        if (!hasNPC)
        {
            GameModeManager.Instance.SwitchToRealTimeMode();
            StopAllCoroutines(); // 모든 코루틴을 중지하여 턴 루프 종료
        }
    }

    // 턴제 모드를 시작하는 함수
    public void StartTurnBasedMode()
    {
        turnTakers.Clear();  // 기존의 턴 참여자 목록 초기화
        currentTurnIndex = 0;

        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // 플레이어 등록
        var playerTurnTaker = playerObject.GetComponent<ITurnTaker>();
        if (playerTurnTaker != null)
        {
            RegisterTurnTaker(playerTurnTaker);
        }

        // NPC 등록
        var npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (var npcObject in npcs)
        {
            var npcTurnTaker = npcObject.GetComponent<ITurnTaker>();
            if (npcTurnTaker != null && IsNpcInRangeOfPlayer(npcTurnTaker, maxDistanceFromPlayer))
            {
                RegisterTurnTaker(npcTurnTaker);
            }
        }

        if (turnTakers.Count > 0)
        {
            StartCoroutine(TurnRoutine());
            Debug.Log("Turn-based mode started with turn takers.");
        }
        else
        {
            GameModeManager.Instance?.SwitchToRealTimeMode();
            Debug.Log("No Turn Takers in range. Switched to real-time mode.");
        }
    }

    // 디버깅용으로 현재 턴 참여자를 출력하는 함수
    public void DebugTurnTakers()
    {
        Debug.Log("Current Turn Takers:");
        foreach (var turnTaker in turnTakers)
        {
            var monoBehaviour = turnTaker as MonoBehaviour;
            if (monoBehaviour != null)
            {
                Debug.Log($"- {monoBehaviour.gameObject.name}");
            }
            else
            {
                Debug.Log("- Null or non-MonoBehaviour TurnTaker");
            }
        }
    }

    // 플레이어 외에 턴 참여자가 남아있는지 확인하는 함수
    private void CheckForRemainingTurnTakers()
    {
        bool hasOtherTurnTakers = false;

        foreach (var turnTaker in turnTakers)
        {
            if (!(turnTaker is PlayerTurnManager))
            {
                hasOtherTurnTakers = true;
                break;
            }
        }

        // 다른 턴 참여자가 없으면 실시간 모드로 전환
        if (!hasOtherTurnTakers)
        {
            GameModeManager.Instance.SwitchToRealTimeMode();
            StopAllCoroutines();
            Debug.Log("No more NPCs. Switching to real-time mode.");
        }
    }

    // 턴을 관리하는 코루틴
    private IEnumerator TurnRoutine()
    {
        while (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            yield return StartCoroutine(TakeTurns());
        }
    }

    // 턴 참여자가 턴을 차례대로 수행하는 함수
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

        // 현재 턴의 주체가 플레이어인지 확인하여 GameStateManager에 알림
        bool isPlayerTurn = CurrentTurnTaker is PlayerTurnManager;
        GameStateManager.Instance.SetPlayerTurnActive(isPlayerTurn);

        CurrentTurnTaker.StartTurn();
        Debug.Log($"{CurrentTurnTaker.Name}의 턴 시작");

        while (!CurrentTurnTaker.IsTurnComplete)
        {
            yield return null;
        }

        CurrentTurnTaker.EndTurn();
        Debug.Log($"{CurrentTurnTaker.Name}의 턴 종료");

        currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;
        CheckForRealTimeMode();
    }
}

    // 다음 턴으로 넘어가는 함수
    public void NextTurn()
    {
        StartCoroutine(CheckForNearbyNPCsAndEndTurnIfNone());
        currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;
        DebugTurnTakers(); // 현재 턴 참여자 디버깅 정보 출력

        if (turnTakers.Count > 0)
        {
            CurrentTurnTaker = turnTakers[currentTurnIndex];

            if (!(CurrentTurnTaker is PlayerTurnManager) && !IsNpcInRangeOfPlayer(CurrentTurnTaker, maxDistanceFromPlayer))
            {
                UnregisterTurnTaker(CurrentTurnTaker);
                NextTurn();  // 다음 턴으로 넘어감
                return;
            }

            if (CurrentTurnTaker != null)
            {
                // NPC일 경우 플레이어 이동 비활성화
                if (!(CurrentTurnTaker is PlayerTurnManager))
                {
                    var player = FindObjectOfType<PlayerTurnManager>();
                    if (player != null)
                    {
                        player.DisableMovement();
                    }
                }
                else
                {
                    var player = FindObjectOfType<PlayerTurnManager>();
                    if (player != null)
                    {
                        player.EnableMovement();
                    }
                }

                CurrentTurnTaker.StartTurn();
                

                CheckForRealTimeMode();
            }
        }

        CheckForRealTimeMode();
        CheckForRemainingTurnTakers();
    }

    // 턴제 모드를 종료하는 함수
    public void EndTurnBasedMode()
    {
        turnTakers.Clear();
        currentTurnIndex = 0;
        GameModeManager.Instance.SwitchToRealTimeMode();
        StopAllCoroutines();
        Debug.Log("Turn-based mode ended, switched to real-time mode.");
    }

    // NPC가 없는지 확인하고 턴제 모드 종료하는 코루틴
    private IEnumerator CheckForNearbyNPCsAndEndTurnIfNone()
    {
        yield return new WaitForSeconds(3f); // 3초 대기

        bool hasNearbyNPCs = false;

        foreach (var turnTaker in TurnManager.Instance.TurnTakers)
        {
            if (!(turnTaker is PlayerTurnManager) && TurnManager.Instance.IsNpcInRangeOfPlayer(turnTaker, TurnManager.Instance.maxDistanceFromPlayer))
            {
                hasNearbyNPCs = true;
                break;
            }
        }

        if (!hasNearbyNPCs)
        {
            Debug.Log("No NPCs nearby after 3 seconds. Ending turn-based mode.");
            TurnManager.Instance.EndTurnBasedMode();
        }
    }

    // 남은 NPC가 있는지 확인하는 함수
    private void CheckForRemainingNPCs()
    {
        bool hasNPC = false;

        foreach (var turnTaker in turnTakers)
        {
            if (!(turnTaker is PlayerTurnManager))
            {
                hasNPC = true;
                break;
            }
        }

        if (!hasNPC)
        {
            Debug.Log("No NPCs remaining. Switching to real-time mode.");
            GameModeManager.Instance.SwitchToRealTimeMode();
            StopAllCoroutines();
        }
    }

    // 랜덤 턴 참여자를 반환하는 함수
    public ITurnTaker GetRandomTurnTaker()
    {
        if (turnTakers.Count == 0)
            return null;

        int randomIndex = Random.Range(0, turnTakers.Count);
        return turnTakers[randomIndex];
    }

    // 첫 턴 참여자를 설정하는 함수
    public void SetFirstTurnTaker(ITurnTaker turnTaker)
    {
        currentTurnIndex = turnTakers.IndexOf(turnTaker);
    }
}
