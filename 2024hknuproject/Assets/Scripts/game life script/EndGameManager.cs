using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    private StageManager stagemanager;
    public GameObject gameEndPanel; // 게임 오버 패널
    public TextMeshProUGUI messageText; // 게임 오버 메시지
    private bool canPressButton = false; // 버튼 입력 가능 상태
    private GameObject player; // 플레이어 오브젝트
    public GameObject fireworkPrefab; // 폭죽 파티클 프리팹
    private void Start()
    {
        gameEndPanel.SetActive(false); // 게임 오버 패널 초기 비활성화
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public IEnumerator ShowGameOverPanel(string message)
    {// 2초 대기 후 게임 오버 패널 표시
        PlayFireworkEffectAtPlayer();
        yield return new WaitForSeconds(2f);
        CycleManager.currentStage = 1;
        stagemanager = FindObjectOfType<StageManager>();
        stagemanager.StageReset();

        

        gameEndPanel.SetActive(true); // 게임 오버 패널 표시
        messageText.text = message;

        StartCoroutine(EnableButtonAfterDelay()); // 일정 시간 후 버튼 활성화
    }

    private IEnumerator EnableButtonAfterDelay()
    {
        
        yield return new WaitForSeconds(2f); // 5초 대기
        canPressButton = true;
        messageText.text += "Press any key to return to main menu"; // 안내 메시지 추가
    }

    private void Update()
    {
        if (canPressButton && Input.anyKeyDown)
        {
            stagemanager = FindObjectOfType<StageManager>();
            
            
            SceneManager.LoadScene("mainmenu"); // 메인 메뉴로 이동
        }
    }
    private void PlayFireworkEffectAtPlayer()
    {
        if (player != null)
        {

            // 플레이어 위치에 폭죽 파티클 생성
            Vector3 particlePosition = new Vector3(player.transform.position.x, player.transform.position.y, -5f);
            GameObject firework = Instantiate(fireworkPrefab, particlePosition, Quaternion.Euler(-90, 0, 0));
            Destroy(firework, 5f); // 파티클 재생 후 2초 뒤 제거
        }
        else
        {
            Debug.LogWarning("Player not found. Cannot play firework effect.");
        }
    }

}
