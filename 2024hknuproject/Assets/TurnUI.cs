using UnityEngine;
using TMPro;

public class TurnUI : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    void Update()
    {
        // GameModeManager.Instance �Ǵ� TurnManager.Instance�� null���� Ȯ��
        if (GameModeManager.Instance == null || TurnManager.Instance == null)
        {
            return;
        }

        // currentMode�� CurrentTurnTaker�� null���� Ȯ��
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
        // �� �ؽ�Ʈ�� ���
        if (turnText != null)
        {
            turnText.text = "";
        }
    }
}
