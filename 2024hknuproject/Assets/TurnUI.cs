using UnityEngine;
using TMPro;

public class TurnUI : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    void Update()
    {
        // GameModeManager.Instance 또는 TurnManager.Instance가 null인지 확인
        if (GameModeManager.Instance == null || TurnManager.Instance == null)
        {
            return;
        }

        // currentMode와 CurrentTurnTaker가 null인지 확인
        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased)
        {
            var currentTurnTaker = TurnManager.Instance.CurrentTurnTaker;
            if (currentTurnTaker != null && currentTurnTaker is Object && currentTurnTaker as MonoBehaviour != null)
            {
                turnText.text = $"{currentTurnTaker.Name}'s Turn";
            }
            else
            {
                turnText.text = "No active turn taker";
            }
        }
        else
        {
            turnText.text = "";
        }
    }

    public void ClearTurnText()
    {
        // 턴 텍스트를 비움
        if (turnText != null)
        {
            turnText.text = "";
        }
    }
}
