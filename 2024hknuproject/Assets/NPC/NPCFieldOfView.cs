using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFieldOfView : MonoBehaviour
{
    public float viewDistance = 5.0f; // �þ� �Ÿ�
    public float viewAngle = 90.0f; // �þ� ����
    public Vector3 viewDirection = Vector3.right; // NPC�� �ٶ󺸴� ����
    public LayerMask playerLayer; // �÷��̾� ���̾�

    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found. Please ensure the player has the tag 'Player'.");
        }
    }

    void Update()
    {
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.RealTime)
        {
            CheckPlayerInSight();
        }
    }

    void CheckPlayerInSight()
    {
        if (player == null) return;

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // �÷��̾�� NPC ������ ���� ���
        float angleToPlayer = Vector3.Angle(viewDirection, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (angleToPlayer < viewAngle / 2 && distanceToPlayer < viewDistance)
        {
            // �÷��̾ �þ߿� �ִ��� Ȯ���ϱ� ���� ����ĳ��Ʈ
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, viewDistance, playerLayer);
            Debug.DrawLine(transform.position, transform.position + (Vector3)(directionToPlayer * viewDistance), Color.red);

            if (hit.collider != null)
            {
                Debug.Log($"Hit: {hit.collider.name}");
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player in sight!");
                    GameModeManager.Instance.SwitchToTurnBasedMode(); // �� ��� ���� ��ȯ
                    return;
                }
                else
                {
                    Debug.Log("Hit, but not Player");
                }
            }
            else
            {
                Debug.Log("No hit detected");
            }
        }
    }

    // �þ� �ð�ȭ�� ���� ����� �׸���
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 rightBoundary = Quaternion.Euler(0, 0, viewAngle / 2) * viewDirection * viewDistance;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -viewAngle / 2) * viewDirection * viewDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);

        Gizmos.color = Color.green;
        Vector3 viewAngleA = Quaternion.Euler(0, 0, viewAngle / 2) * viewDirection * viewDistance;
        Vector3 viewAngleB = Quaternion.Euler(0, 0, -viewAngle / 2) * viewDirection * viewDistance;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB);
    }
}
