using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private List<ITurnTaker> turnTakers = new List<ITurnTaker>();
    private int currentTurnIndex;
    public ITurnTaker CurrentTurnTaker { get; private set; }

    public List<ITurnTaker> TurnTakers => turnTakers; // TurnTakers ����Ʈ�� ���� ������

    public string npcLayerName = "npcLayer"; // NPC ���̾� �̸�
    private int npcLayer;
    public float maxDistanceFromPlayer = 15f; // �÷��̾�� NPC ���� �ִ� �Ÿ�

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            npcLayer = LayerMask.NameToLayer(npcLayerName); // NPC ���̾� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterTurnTaker(ITurnTaker turnTaker)
    {
        // �ߺ� ��� ����: ����Ʈ�� �̹� �ִ��� Ȯ��
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

            // currentTurnIndex�� ������
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
            StopAllCoroutines(); // ��� �ڷ�ƾ�� �����Ͽ� �� ��ƾ�� ������
        }
    }


    public void StartTurnBasedMode()
    {
        // �÷��̾��� ��ġ�� �������� ��ó�� �ִ� NPC�� �� ����Ŀ ����Ʈ�� ���
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

        // �÷��̾ ����Ʈ�� �߰�
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
    // �÷��̾ ������ �ٸ� TurnTaker�� �ִ��� Ȯ��
    bool hasOtherTurnTakers = false;

    foreach (var turnTaker in turnTakers)
    {
        if (!(turnTaker is PlayerTurnManager))
        {
            hasOtherTurnTakers = true;
            break;
        }
    }

    // �ٸ� TurnTaker�� ������ �ǽð� ���� ��ȯ
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

            // NPC�� ���� ���۵� �� �Ÿ� üũ �� ����
            if (!(CurrentTurnTaker is PlayerTurnManager) && !IsNpcInRangeOfPlayer(CurrentTurnTaker, maxDistanceFromPlayer))
            {
                UnregisterTurnTaker(CurrentTurnTaker);
                continue;
            }
            if (CurrentTurnTaker is PlayerTurnManager)
            {
                CheckForRemainingNPCs();
            }

            // CurrentTurnTaker�� ��ȿ���� Ȯ����
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
            Debug.Log($"{CurrentTurnTaker.Name}�� �� ����"); // ������ ������ �α� ���

            // NPC ���̸� �÷��̾� ������ ��Ȱ��ȭ
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
            Debug.Log($"{CurrentTurnTaker.Name}�� �� ����"); // ������ ���� ����Ǿ����� �α� ���

            currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;

            // ���� �� ���� �� NPC�� �ִ��� Ȯ��
            CheckForRealTimeMode();
        }
    }



public void NextTurn()
{
        StartCoroutine(CheckForNearbyNPCsAndEndTurnIfNone());
        currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;
        // �����: ���� ��ϵ� TurnTaker ��� ���
        DebugTurnTakers();
        // ���� �� ����Ŀ�� �������� ������ NPC �˻� ����
        if (turnTakers.Count > 0)
    {
        CurrentTurnTaker = turnTakers[currentTurnIndex];

        if (!(CurrentTurnTaker is PlayerTurnManager) && !IsNpcInRangeOfPlayer(CurrentTurnTaker, maxDistanceFromPlayer))
        {
            UnregisterTurnTaker(CurrentTurnTaker);
            NextTurn();  // ��������� ���� ������ �Ѿ��
            return;
        }

        if (CurrentTurnTaker != null)
        {
            // NPC ���̸� �÷��̾� ������ ��Ȱ��ȭ
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
            Debug.Log($"{CurrentTurnTaker.Name}�� �� ����");

            // ���� ���۵� �� NPC�� �ִ��� Ȯ��
            CheckForRealTimeMode();
        }
    }

    // NPC�� ������ Ȯ��
    CheckForRealTimeMode();

    // �� ���� �� ����Ʈ �˻�
    CheckForRemainingTurnTakers();
}

    public void EndTurnBasedMode()
    {
        // ��� ������Ŀ ����
        turnTakers.Clear();
        currentTurnIndex = 0;

        // �ǽð� ���� ��ȯ
        GameModeManager.Instance.SwitchToRealTimeMode();
        StopAllCoroutines();

        Debug.Log("Turn-based mode ended, switched to real-time mode.");
    }
    private IEnumerator CheckForNearbyNPCsAndEndTurnIfNone()
    {
        yield return new WaitForSeconds(3f); // 3�� ���

        bool hasNearbyNPCs = false;

        foreach (var turnTaker in TurnManager.Instance.TurnTakers)
        {
            if (!(turnTaker is PlayerTurnManager) && TurnManager.Instance.IsNpcInRangeOfPlayer(turnTaker, TurnManager.Instance.maxDistanceFromPlayer))
            {
                hasNearbyNPCs = true;
                break;
            }
        }

        // ���� 3�� �Ŀ��� ��ó�� NPC�� ���ٸ� ���� ��� ����
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

        // ���� NPC�� �ϳ��� ������ �ǽð� ���� ��ȯ
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
