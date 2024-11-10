
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ActionpointText : MonoBehaviour
{
    public PlayerTurnManager PTM; 
    public TextMeshProUGUI APText; 
    public Slider APBar;

    void Update()
    {
        if (PTM != null)
        {
            
            if (APText != null)
            {
                APText.text = "AP: " + PTM.GetCurrentAP().ToString();
            }
            
            if (APBar != null)
            {
                APBar.value = PTM.GetCurrentAP();
            }
        }
    }
}
