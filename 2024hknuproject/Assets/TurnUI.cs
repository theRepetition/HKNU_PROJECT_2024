using UnityEngine;
using UnityEngine.UI;  // Image를 사용하기 위한 네임스페이스
using TMPro;

public class TurnUI : MonoBehaviour
{
    public TextMeshProUGUI turnText;
    public Image turnImage;

    void Update()
    {
        if (GameModeManager.Instance == null || TurnManager.Instance == null)
        {
            return;
        }

        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            turnImage.gameObject.SetActive(true); // 턴제 모드일 때 turnImage 활성화

            var currentTurnTaker = TurnManager.Instance.CurrentTurnTaker;
            if (currentTurnTaker != null && currentTurnTaker is MonoBehaviour)
            {
                string turnTakerTag = ((MonoBehaviour)currentTurnTaker).tag;
                if (turnTakerTag == "Player")
                {
                    turnText.text = "플레이어의 턴";
                }
                else
                {
                    turnText.text = "NPC의 턴";
                }
            }
            else
            {
                turnText.text = "No active turn taker";
            }
        }
        else
        {
            turnImage.gameObject.SetActive(false); // 턴제가 아닐 때 turnImage 비활성화
            turnText.text = "";
        }
    }

    public void ClearTurnText()
    {
        if (turnText != null)
        {
            turnText.text = "";
        }
    }
}
