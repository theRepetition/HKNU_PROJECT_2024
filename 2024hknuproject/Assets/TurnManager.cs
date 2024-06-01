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

            // currentTurnIndex�� ������
            if (indexToRemove <= currentTurnIndex && currentTurnIndex > 0)
            {
                currentTurnIndex--;
            }
        }

        Debug.Log("UnregisterTurnTaker finished.");
        // NPC�� ������ �ǽð� ���� ��ȯ��
        CheckForRealTimeMode();
    }

    public void CheckForRealTimeMode()
    {
        Debug.Log("��� Ȯ��");

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
            Debug.Log("���� ����");
            GameModeManager.Instance.SwitchToRealTimeMode();
            StopAllCoroutines(); // ��� �ڷ�ƾ�� �����Ͽ� �� ��ƾ�� ������
        }
        else
        {
            Debug.Log("���� ����");
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

                // NPC�� ������ �ǽð� ���� ��ȯ��
                CheckForRealTimeMode();
                continue;
            }

            CurrentTurnTaker.StartTurn();

            while (!CurrentTurnTaker.IsTurnComplete)
            {
                yield return null;
            }

            CurrentTurnTaker.EndTurn();

            // NPC�� ������ �ǽð� ���� ��ȯ��
            CheckForRealTimeMode();

            currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;

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
                CurrentTurnTaker.StartTurn();
            }
        }

        // NPC�� ������ Ȯ��
        CheckForRealTimeMode();
    }
}
