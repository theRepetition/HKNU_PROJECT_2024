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
    public float maxDistanceFromPlayer = 15f; // 플레이어와 NPC 간의 최대 거리

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
    private bool IsNpcInRangeOfPlayer(ITurnTaker npc, float range)
    {
        var npcObject = (npc as MonoBehaviour)?.gameObject;
        var playerObject = GameObject.FindGameObjectWithTag("Player");

        if (npcObject == null || playerObject == null)
            return false;

        return Vector2.Distance(npcObject.transform.position, playerObject.transform.position) <= range;
    }


    public void UnregisterTurnTaker(ITurnTaker turnTaker)
    {
        if (turnTakers.Contains(turnTaker))
        {
            int indexToRemove = turnTakers.IndexOf(turnTaker);
            turnTakers.Remove(turnTaker);

            // currentTurnIndex를 조정함
            if (indexToRemove <= currentTurnIndex && currentTurnIndex > 0)
            {
                currentTurnIndex--;
            }
        }
    }

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
            StopAllCoroutines(); // 모든 코루틴을 중지하여 턴 루틴을 종료함
        }
    }


    public void StartTurnBasedMode()
    {
        // 플레이어의 위치를 기준으로 근처에 있는 NPC만 턴 테이커 리스트에 등록
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        var npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (var npcObject in npcs)
        {
            var npcTurnTaker = npcObject.GetComponent<ITurnTaker>();
            if (npcTurnTaker != null)
            {
                float distanceToPlayer = Vector2.Distance(npcObject.transform.position, playerObject.transform.position);
                if (distanceToPlayer <= maxDistanceFromPlayer)
                {
                    RegisterTurnTaker(npcTurnTaker);
                }
            }
        }

        // 플레이어도 리스트에 추가
        var playerTurnTaker = playerObject.GetComponent<ITurnTaker>();
        if (playerTurnTaker != null)
        {
            RegisterTurnTaker(playerTurnTaker);
        }

        

        StartCoroutine(TurnRoutine());
    }


    


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
private void CheckForRemainingTurnTakers()
{
    // 플레이어를 제외한 다른 TurnTaker가 있는지 확인
    bool hasOtherTurnTakers = false;

    foreach (var turnTaker in turnTakers)
    {
        if (!(turnTaker is PlayerTurnManager))
        {
            hasOtherTurnTakers = true;
            break;
        }
    }

    // 다른 TurnTaker가 없으면 실시간 모드로 전환
    if (!hasOtherTurnTakers)
    {
        GameModeManager.Instance.SwitchToRealTimeMode();
        StopAllCoroutines();
        Debug.Log("No more NPCs. Switching to real-time mode.");
    }
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

            // NPC의 턴이 시작될 때 거리 체크 후 제거
            if (!(CurrentTurnTaker is PlayerTurnManager) && !IsNpcInRangeOfPlayer(CurrentTurnTaker, maxDistanceFromPlayer))
            {
                UnregisterTurnTaker(CurrentTurnTaker);
                continue;
            }
            if (CurrentTurnTaker is PlayerTurnManager)
            {
                CheckForRemainingNPCs();
            }

            // CurrentTurnTaker가 유효한지 확인함
            if (CurrentTurnTaker == null || CurrentTurnTaker as MonoBehaviour == null)
            {
                turnTakers.RemoveAt(currentTurnIndex);
                if (currentTurnIndex >= turnTakers.Count)
                {
                    currentTurnIndex = 0;
                }

                continue;
            }

            CurrentTurnTaker.StartTurn();
            Debug.Log($"{CurrentTurnTaker.Name}의 턴 시작"); // 누구의 턴인지 로그 출력

            // NPC 턴이면 플레이어 움직임 비활성화
            if (!(CurrentTurnTaker is PlayerTurnManager))
            {
                var player = FindObjectOfType<PlayerTurnManager>();
                if (player != null)
                {
                    player.DisableMovement();
                }
            }

            while (!CurrentTurnTaker.IsTurnComplete)
            {
                yield return null;
            }

            CurrentTurnTaker.EndTurn();
            Debug.Log($"{CurrentTurnTaker.Name}의 턴 종료"); // 누구의 턴이 종료되었는지 로그 출력

            currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;

            // 다음 턴 시작 시 NPC가 있는지 확인
            CheckForRealTimeMode();
        }
    }



public void NextTurn()
{
        StartCoroutine(CheckForNearbyNPCsAndEndTurnIfNone());
        currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;
        // 디버그: 현재 등록된 TurnTaker 목록 출력
        DebugTurnTakers();
        // 현재 턴 테이커가 존재하지 않으면 NPC 검사 수행
        if (turnTakers.Count > 0)
    {
        CurrentTurnTaker = turnTakers[currentTurnIndex];

        if (!(CurrentTurnTaker is PlayerTurnManager) && !IsNpcInRangeOfPlayer(CurrentTurnTaker, maxDistanceFromPlayer))
        {
            UnregisterTurnTaker(CurrentTurnTaker);
            NextTurn();  // 재귀적으로 다음 턴으로 넘어가기
            return;
        }

        if (CurrentTurnTaker != null)
        {
            // NPC 턴이면 플레이어 움직임 비활성화
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
            Debug.Log($"{CurrentTurnTaker.Name}의 턴 시작");

            // 턴이 시작될 때 NPC가 있는지 확인
            CheckForRealTimeMode();
        }
    }

    // NPC가 없는지 확인
    CheckForRealTimeMode();

    // 턴 종료 후 리스트 검사
    CheckForRemainingTurnTakers();
}

    public void EndTurnBasedMode()
    {
        // 모든 턴테이커 제거
        turnTakers.Clear();
        currentTurnIndex = 0;

        // 실시간 모드로 전환
        GameModeManager.Instance.SwitchToRealTimeMode();
        StopAllCoroutines();

        Debug.Log("Turn-based mode ended, switched to real-time mode.");
    }
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

        // 만약 3초 후에도 근처에 NPC가 없다면 턴제 모드 종료
        if (!hasNearbyNPCs)
        {
            Debug.Log("No NPCs nearby after 3 seconds. Ending turn-based mode.");
            TurnManager.Instance.EndTurnBasedMode();
        }
    }
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

        // 만약 NPC가 하나도 없으면 실시간 모드로 전환
        if (!hasNPC)
        {
            Debug.Log("No NPCs remaining. Switching to real-time mode.");
            GameModeManager.Instance.SwitchToRealTimeMode();
            StopAllCoroutines();
        }
    }
    public ITurnTaker GetRandomTurnTaker()
    {
        if (turnTakers.Count == 0)
            return null;

        int randomIndex = Random.Range(0, turnTakers.Count);
        return turnTakers[randomIndex];
    }

    public void SetFirstTurnTaker(ITurnTaker turnTaker)
    {
        currentTurnIndex = turnTakers.IndexOf(turnTaker);
    }
}
