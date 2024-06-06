using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        TurnManager.Instance.RegisterTurnTaker(this);
        health = GetComponent<Health>();
        Debug.Log($"NPC {gameObject.name} registered as TurnTaker.");
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
            StartCoroutine(NPCTurnRoutine());
        }
        else
        {
            EndTurn();
        }
    }

    private IEnumerator NPCTurnRoutine()
    {
        float turnDuration = 3.0f;
        float elapsed = 0f;

        while (elapsed < turnDuration)
        {
            if (currentActionPoints > 0)
            {
                Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                MoveCharacter(direction);
                yield return new WaitForSeconds(1.0f);
                elapsed += 1.0f;
            }
            else
            {
                yield return null;
            }
        }

        EndTurn();
    }

    private void MoveCharacter(Vector2 direction)
    {
        Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        float distance = Vector2.Distance(rb.position, newPosition);

        if (currentActionPoints >= distance)
        {
            rb.MovePosition(newPosition);
            currentActionPoints -= Mathf.CeilToInt(distance);
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
        Debug.Log($"NPC {gameObject.name} died and is being unregistered.");
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
