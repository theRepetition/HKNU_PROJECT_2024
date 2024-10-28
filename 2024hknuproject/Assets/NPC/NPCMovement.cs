using System.Collections;
using UnityEngine;

public class NPCMovement : MonoBehaviour, ITurnTaker
{
    public float moveSpeed = 3f;
    public int maxActionPoints = 5;
    private int currentActionPoints;
    private bool isTurnComplete;
    private Rigidbody2D rb;
    private Health health;
    public LayerMask playerLayer; // �÷��̾� ���̾�
    public NPCCombatAI npcCombatAI;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        TurnManager.Instance.RegisterTurnTaker(this);
        health = GetComponent<Health>();
        npcCombatAI = GetComponent<NPCCombatAI>();
    }

    private void Update()
    {
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.RealTime)
        {
            CheckForPlayer();
        }
        if (health != null && health.GetCurrentHealth() <= 0)
        {
            Die();
        }
    }

    public void StartTurn()
    {
        if (health != null && health.GetCurrentHealth() > 0)
        {
            currentActionPoints = maxActionPoints;
            isTurnComplete = false;
            npcCombatAI.StartTurn();
        }
        
    }

    public void EndTurn()
    {
        isTurnComplete = true;
        TurnManager.Instance.NextTurn();
    }

    public bool IsTurnComplete => isTurnComplete;

    public string Name => gameObject.name;

    private void CheckForPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 5.0f, playerLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            GameModeManager.Instance.SwitchToTurnBasedMode();
        }
    }

    private void Die()
    {
        TurnManager.Instance.UnregisterTurnTaker(this);

        // NPC�� ���� �� ó��
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.1f); // �ణ�� ����

        // ���� �� ����Ŀ�� �� NPC�� ��� ���� �ѱ�
        if (TurnManager.Instance.CurrentTurnTaker == this)
        {
            TurnManager.Instance.NextTurn();
        }
        else
        {
            // NPC�� �׾��� �� �÷��̾��� ���̸� �÷��̾� �� ����
            TurnManager.Instance.CheckForRealTimeMode();
        }

        Destroy(gameObject); // �˻簡 ���� �� ������Ʈ�� ����
    }
}
