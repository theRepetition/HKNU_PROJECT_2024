using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthText : MonoBehaviour
{
    public Health playerHealth; 
    public TextMeshProUGUI healthText; 
    public Slider healthBar; 

    void Update()
    {
        if (playerHealth != null)
        {
            
            if (healthText != null)
            {
                healthText.text = "Health: " + playerHealth.GetCurrentHealth().ToString();
            }
           
            if (healthBar != null)
            {
                healthBar.value = playerHealth.GetCurrentHealth();
            }
        }
    }
}
