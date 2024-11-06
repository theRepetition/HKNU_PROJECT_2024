using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class PlayerHealth : Health
{
    public StageManager stageManager;         // 스테이지 관리 스크립트 참조
    public GameObject deathScreenUI;          // 죽음 화면 UI (최종 스테이지 표시 및 버튼 안내)
    public TMP_Text stageText;                // 최종 스테이지를 표시하는 TextMeshPro 텍스트
    private GameoverManager gameoverManager;
    private bool isWaitingForInput = false;   // 입력 대기 상태 확인
    private void Start()
    {
        currentHealth = maxHealth;
        gameoverManager = FindObjectOfType<GameoverManager>(); // GameoverManager 찾기
    }
    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
        Debug.Log("Player has died");

        gameoverManager.ShowGameOverPanel("");
    }
    public void Heal(int amount) //체력 회복
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Player healed by {amount}. Current Health: {currentHealth}");
    }
    // 최대 체력 증가 메서드
    public void IncreaseMaxHealth(int amount)
    {
        // 최대 체력 증가 전의 현재 체력 비율 계산
        float healthPercentage = (float)currentHealth / maxHealth;

        // 최대 체력 증가
        maxHealth += amount;

        // 현재 체력을 새로운 최대 체력의 동일 비율로 회복
        currentHealth = Mathf.FloorToInt(maxHealth * healthPercentage);

        Debug.Log($"Max Health increased by {amount}. New Max Health: {maxHealth}. Current Health: {currentHealth}");
    }

   

        
    

    private void Update()
    {

    }
}
