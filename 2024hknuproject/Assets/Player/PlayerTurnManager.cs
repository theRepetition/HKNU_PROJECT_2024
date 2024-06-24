using UnityEngine;

public class PlayerTurnManager : MonoBehaviour, ITurnTaker
{
    public int maxActionPoints = 10; // �ִ� �ൿ��
    private int currentActionPoints;
    private bool isTurnComplete = false;
    public PlayerMovement playerMovement;
    private PlayerCombat playerCombat;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        TurnManager.Instance.RegisterTurnTaker(this);
    }

    void Update()
    {
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && TurnManager.Instance.CurrentTurnTaker == this)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isTurnComplete && playerCombat.ProjectilesOnField == 0)
            {
                EndTurn();
            }
        }
    }

    public void StartTurn()
    {
        currentActionPoints = maxActionPoints; // ���� ���۵� �� �ൿ�� �ʱ�ȭ
        isTurnComplete = false; // �� ���� �� �ʱ�ȭ

        playerMovement.EnableMovement(); // �÷��̾��� ������ Ȱ��ȭ
        playerCombat.ResetProjectilesFired(); // �߻�ü �� �ʱ�ȭ
        Debug.Log("�� �� ����");
    }

    public void EndTurn()
    {
        playerMovement.DisableMovement(); // �÷��̾��� ������ ��Ȱ��ȭ
        isTurnComplete = true; // �� �Ϸ� ����
        Debug.Log("�� ����");
        TurnManager.Instance.NextTurn(); // �� ���� �� ���� ������ ��ȯ
    }

    public bool IsTurnComplete => isTurnComplete; // �� �Ϸ� ����

    public string Name => gameObject.name; // �̸� ��ȯ

    public void DisableMovement()
    {
        playerMovement.DisableMovement();
    }

    public void EnableMovement()
    {
        playerMovement.EnableMovement();
    }

    public int CurrentActionPoints
    {
        get => currentActionPoints;
        set => currentActionPoints = value;
    }
}
