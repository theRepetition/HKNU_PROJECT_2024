using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class PlayerHealth : Health
{
    public StageManager stageManager;         // 스테이지 관리 스크립트 참조
    public GameObject deathScreenUI;          // 죽음 화면 UI (최종 스테이지 표시 및 버튼 안내)
    public TMP_Text stageText;                // 최종 스테이지를 표시하는 TextMeshPro 텍스트

    private bool isWaitingForInput = false;   // 입력 대기 상태 확인

    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
        Debug.Log("Player has died");

        // 최종 스테이지 표시 설정
        ShowDeathScreen();
    }

    private void ShowDeathScreen()
    {
        
        stageText.text = "Final Stage: " + stageManager.CurrentStage.ToString();   // 최종 스테이지 표시

        // 죽음 화면 UI 활성화
        deathScreenUI.SetActive(true);

        // 입력 대기 상태 설정
        isWaitingForInput = true;
    }

    private void Update()
    {
        // 플레이어가 죽은 후, 아무 버튼이나 눌렀을 때 씬 전환
        if (isWaitingForInput && Input.anyKeyDown)
        {
            SceneManager.LoadScene("mainmenu"); // 전환할 씬 이름으로 변경
        }
    }
}
