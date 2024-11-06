using UnityEngine;

public class PlayerTurnManager : MonoBehaviour, ITurnTaker
{
    public int maxActionPoints = 10; // 최대 행동 포인트
    public int currentActionPoints; // 현재 행동 포인트
    private bool isTurnComplete = false; // 턴 완료 여부
    public PlayerMovement playerMovement; // 플레이어의 이동 스크립트
    private PlayerCombat playerCombat; // 플레이어의 전투 스크립트

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>(); // PlayerMovement 스크립트를 가져옴
        playerCombat = GetComponent<PlayerCombat>(); // PlayerCombat 스크립트를 가져옴
        TurnManager.Instance.RegisterTurnTaker(this); // TurnManager에 이 오브젝트를 턴 참여자로 등록
    }

    void Update()
    {
        
    }

    // 턴을 시작하는 함수
    public void StartTurn()
    {
        currentActionPoints = maxActionPoints; // 턴 시작 시 행동 포인트를 최대치로 초기화
        isTurnComplete = false; // 턴이 완료되지 않은 상태로 설정

        GameStateManager.Instance.SetPlayerTurn(true);
        playerCombat.ResetProjectilesFired(); // 발사체 수 초기화
        Debug.Log("플레이어 턴 시작");
    }

    // 턴을 종료하는 함수
    public void EndTurn()
    {
        GameStateManager.Instance.SetPlayerTurn(false);
        isTurnComplete = true; // 턴 완료 상태로 설정
        
        Debug.Log("플레이어 턴 종료");
        TurnManager.Instance.NextTurn(); // 다음 턴으로 이동
    }

    // 턴 완료 상태를 반환
    public bool IsTurnComplete => isTurnComplete;

    // 게임 오브젝트의 이름을 반환
    public string Name => gameObject.name;

    // 이동 비활성화 함수
    public void DisableMovement()
    {
        playerMovement.DisableMovement();
    }

    // 이동 활성화 함수
    public void EnableMovement()
    {
        playerMovement.EnableMovement();
    }

    // 현재 행동 포인트를 가져오거나 설정
    public int CurrentActionPoints
    {
        get => currentActionPoints;
        set => currentActionPoints = value;
    }
    public void IncreaseMaxActionPoints(int amount)
    {
        maxActionPoints += amount;
        currentActionPoints = Mathf.Min(currentActionPoints, maxActionPoints); // 최대 행동력 증가 후 현재 행동력 유지
        Debug.Log($"Max Action Points increased by {amount}. New Max Action Points: {maxActionPoints}");
    }
}
