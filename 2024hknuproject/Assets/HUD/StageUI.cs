using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stage_UI : MonoBehaviour
{
    public StageManager stageManager; // StageManager 참조
    public TextMeshProUGUI stageText; // TextMeshPro를 사용할 경우

    void Start()
    {
        UpdateStageUI(); // 처음 시작할 때 UI 업데이트
    }

    void Update()
    {
        // 계속해서 스테이지 값을 확인하여 UI를 업데이트
        UpdateStageUI();
    }

    private void UpdateStageUI()
    {
        if (stageManager != null && stageText != null)
        {
            stageText.text = "Stage: " + stageManager.CurrentStage.ToString();
        }
    }
}
