using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private bool isGamePaused = false;
    private bool isPauseMenuOpen = false;
    private bool isInventoryOpen = false;
    private bool isRewardUIOpen = false;
    private bool inputBlocked = false;
    private bool isPlayerTurn = false;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdatePlayerReferences();
    }

    // 플레이어 관련 컴포넌트 참조 업데이트
    public void UpdatePlayerReferences()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    public void SetPauseMenuOpen(bool isOpen)
    {
        isPauseMenuOpen = isOpen;
        UpdateGameState();
    }

    public void SetInventoryOpen(bool isOpen)
    {
        isInventoryOpen = isOpen;
        UpdateGameState();
    }

    public void SetRewardUIOpen(bool isOpen)
    {
        isRewardUIOpen = isOpen;
        UpdateGameState();
    }

    public bool IsAnyUIOpen()
    {
        return isPauseMenuOpen || isInventoryOpen || isRewardUIOpen;
    }

    public bool IsPauseMenuOpen()
    {
        return isPauseMenuOpen;
    }

    private void UpdateGameState()
    {
        isGamePaused = isPauseMenuOpen || isInventoryOpen || isRewardUIOpen;

        if (isGamePaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
            StartCoroutine(BlockInputTemporarily());
        }
    }

    public void PauseGame()
    {
        if (playerMovement == null || playerCombat == null) UpdatePlayerReferences();

        if (playerMovement != null) playerMovement.DisableMovement();
        if (playerCombat != null) playerCombat.DisableCombat();

        
    }
    public void SetPlayerTurnActive(bool isPlayerTurn)
    {
        // 현재 턴이 플레이어 턴이면서 UI가 열려 있지 않을 때만 이동 가능
        bool shouldEnableControls = isPlayerTurn && !IsAnyUIOpen();
        if (shouldEnableControls)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    public void ResumeGame()
    {
        // 플레이어의 턴인 경우에만 이동 및 전투 활성화
        if (TurnManager.Instance != null &&
            (GameModeManager.Instance.currentMode != GameModeManager.GameMode.TurnBased ||
            (TurnManager.Instance.CurrentTurnTaker is PlayerTurnManager)))
        {
            if (playerMovement == null || playerCombat == null) UpdatePlayerReferences();

            if (playerMovement != null) playerMovement.EnableMovement();
            if (playerCombat != null) playerCombat.EnableCombat();
        }

        Time.timeScale = 1f;
    }
    public void SetPlayerTurn(bool isTurn)
    {
        isPlayerTurn = isTurn;

        if (isTurn)
        {
            playerMovement?.EnableMovement();
            playerCombat?.EnableCombat();
        }
        else
        {
            playerMovement?.DisableMovement();
            playerCombat?.DisableCombat();
        }
    }
    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    private IEnumerator BlockInputTemporarily()
    {
        inputBlocked = true;
        yield return new WaitForSeconds(0.1f);
        inputBlocked = false;
    }

    public bool IsInputBlocked()
    {
        return inputBlocked;
    }

    // 게임 재시작 (씬 다시 로드)
    public void RestartGame()
    {
        Instance = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
