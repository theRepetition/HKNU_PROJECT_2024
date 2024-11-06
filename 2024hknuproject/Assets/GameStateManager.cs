using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private bool isGamePaused = false;
    private bool isPauseMenuOpen = false;
    private bool isInventoryOpen = false;
    private bool isRewardUIOpen = false;
    private bool inputBlocked = false; // 입력 차단 상태

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

    // 모든 UI가 닫혔을 때만 false 반환
    public bool IsAnyUIOpen()
    {
        return isPauseMenuOpen || isInventoryOpen || isRewardUIOpen;
    }

    // 특정 UI 열림 상태 반환
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
            StartCoroutine(BlockInputTemporarily()); // 입력을 잠시 차단
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        playerMovement?.DisableMovement();
        playerCombat?.DisableCombat();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        playerMovement?.EnableMovement();
        playerCombat?.EnableCombat();
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    // 입력을 잠시 차단하는 코루틴
    private IEnumerator BlockInputTemporarily()
    {
        inputBlocked = true;
        yield return new WaitForSeconds(0.1f); // 0.1초 동안 입력 차단
        inputBlocked = false;
    }

    public bool IsInputBlocked()
    {
        return inputBlocked;
    }
}
