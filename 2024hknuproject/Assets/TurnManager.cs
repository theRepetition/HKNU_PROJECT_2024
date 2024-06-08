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
        turnTakers.Add(turnTaker);
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
            if (monoBehaviour != null && monoBehaviour.gameObject.layer == npcLayer)
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
        currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;

        // ���� �� ����Ŀ�� �������� ������ NPC �˻� ����
        if (turnTakers.Count > 0)
        {
            CurrentTurnTaker = turnTakers[currentTurnIndex];
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
                Debug.Log($"{CurrentTurnTaker.Name}�� �� ����"); // ������ ������ �α� ���

                // ���� ���۵� �� NPC�� �ִ��� Ȯ��
                CheckForRealTimeMode();
            }
        }

        // NPC�� ������ Ȯ��
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
