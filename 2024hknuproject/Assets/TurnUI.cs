using UnityEngine;
using UnityEngine.UI;
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

        // Turn-based 모드에서만 TurnUI를 활성화
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            turnImage.gameObject.SetActive(true);

            var currentTurnTaker = TurnManager.Instance.CurrentTurnTaker;
            if (currentTurnTaker != null && currentTurnTaker is MonoBehaviour turnTakerObj && turnTakerObj != null && turnTakerObj.gameObject != null)
            {
                // 현재 턴이 플레이어인지 NPC인지 확인
                if (turnTakerObj.CompareTag("Player"))
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
                // 현재 턴이 null이거나 유효하지 않은 경우 처리
                turnText.text = "No active turn taker";
            }
        }
        else
        {
            turnImage.gameObject.SetActive(false); // 턴제가 아닐 때 UI 비활성화
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
