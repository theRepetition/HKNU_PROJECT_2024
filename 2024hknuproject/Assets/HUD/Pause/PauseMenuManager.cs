using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;       // 메뉴 패널 오브젝트
    public GameObject howToPlayPanel;  // 플레이 방법 패널 오브젝트
    private bool isGamePaused = false;
    private bool isPauseMenuOpen = false; // 메뉴 열림 상태 변수 추가
    private bool isInventoryOpen = false;
    private bool isRewardUIOpen = false;
    private StageManager stagemanager;
    private void Update()
    {
        // ESC 키 입력 체크
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameStateManager.Instance.IsGamePaused())
                ResumeGame();
            else
                PauseGame();
        }

        // 플레이 방법 패널이 활성화된 경우, 아무 키나 눌렀을 때 닫기
        if (howToPlayPanel.activeSelf && Input.anyKeyDown)
        {
            CloseHowToPlay();
            ResumeGame();
        }
    }
    public bool IsPauseMenuOpen()
    {
        return isPauseMenuOpen;
    }
    public void PauseGame()
    {
        GameStateManager.Instance.SetPauseMenuOpen(true);
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        GameStateManager.Instance.SetPauseMenuOpen(false);
        pauseMenu.SetActive(false);
        howToPlayPanel.SetActive(false);
    }


    public void GoToMainMenu()
    {
        stagemanager = FindObjectOfType<StageManager>();
        GameStateManager.Instance.ResumeGame(); // 시간 재설정
        CycleManager.currentStage = 1;
        stagemanager.StageReset();
        SceneManager.LoadScene("mainmenu"); // 메인 메뉴 씬으로 이동
    }

    public void ExitGame()
    {
        Application.Quit(); // 게임 종료
        Debug.Log("게임이 종료됩니다.");
    }

    public void ShowHowToPlay()
    {
        howToPlayPanel.SetActive(true); // 플레이 방법 패널 활성화
    }

    private void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false); // 플레이 방법 패널 비활성화
    }
}
