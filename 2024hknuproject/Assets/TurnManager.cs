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

    public void RegisterTurnTaker(ITurnTaker turnTaker)
    {
        turnTakers.Add(turnTaker);
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
            if (monoBehaviour != null && monoBehaviour.gameObject.layer == npcLayer)
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
        currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;

        // 현재 턴 테이커가 존재하지 않으면 NPC 검사 수행
        if (turnTakers.Count > 0)
        {
            CurrentTurnTaker = turnTakers[currentTurnIndex];
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
                Debug.Log($"{CurrentTurnTaker.Name}의 턴 시작"); // 누구의 턴인지 로그 출력

                // 턴이 시작될 때 NPC가 있는지 확인
                CheckForRealTimeMode();
            }
        }

        // NPC가 없는지 확인
        CheckForRealTimeMode();
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
