using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthText : MonoBehaviour
{
    public Health playerHealth; // 플레이어의 Health 컴포넌트를 참조
    public TextMeshProUGUI healthText; // Text UI 요소를 참조
    public Slider healthBar; // Slider UI 요소를 참조

    void Update()
    {
        if (playerHealth != null)
        {
            // 텍스트 업데이트
            if (healthText != null)
            {
                healthText.text = "Health: " + playerHealth.GetCurrentHealth().ToString();
            }
            // 슬라이더 업데이트
            if (healthBar != null)
            {
                healthBar.value = playerHealth.GetCurrentHealth();
            }
        }
    }
}
