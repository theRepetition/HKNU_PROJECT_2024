using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthText : MonoBehaviour
{
    public Health playerHealth; // �÷��̾��� Health ������Ʈ�� ����
    public TextMeshProUGUI healthText; // Text UI ��Ҹ� ����
    public Slider healthBar; // Slider UI ��Ҹ� ����

    void Update()
    {
        if (playerHealth != null)
        {
            // �ؽ�Ʈ ������Ʈ
            if (healthText != null)
            {
                healthText.text = "Health: " + playerHealth.GetCurrentHealth().ToString();
            }
            // �����̴� ������Ʈ
            if (healthBar != null)
            {
                healthBar.value = playerHealth.GetCurrentHealth();
            }
        }
    }
}
