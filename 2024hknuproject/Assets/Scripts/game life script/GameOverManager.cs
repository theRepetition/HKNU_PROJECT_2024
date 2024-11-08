using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameoverManager : MonoBehaviour
{
    public GameObject gameEndPanel; // 게임 오버 패널
    public TextMeshProUGUI messageText; // 게임 오버 메시지
    private bool canPressButton = false; // 버튼 입력 가능 상태

    private void Start()
    {
        gameEndPanel.SetActive(false); // 게임 오버 패널 초기 비활성화
    }

    public void ShowGameOverPanel(string message)
    {
        gameEndPanel.SetActive(true); // 게임 오버 패널 표시
        messageText.text = message;
        StartCoroutine(EnableButtonAfterDelay()); // 일정 시간 후 버튼 활성화
    }

    private IEnumerator EnableButtonAfterDelay()
    {
        yield return new WaitForSeconds(2f); // 5초 대기
        canPressButton = true;
        messageText.text += "\n\nGA"; // 안내 메시지 추가
    }

    private void Update()
    {
        if (canPressButton && Input.anyKeyDown)
        {
            SceneManager.LoadScene("mainmenu"); // 메인 메뉴로 이동
        }
    }
}
