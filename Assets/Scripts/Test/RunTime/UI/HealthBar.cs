using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public const int MAKS_HEALTH_VALUE = 25;
    public int currentHealth = 20;
    
    [SerializeField] private Image healthBar;

    private void OnEnable()
    {
        DialogueLoader.OnCallDialogueAction += ChangeHealthBarValue;
    }

    private void OnDisable()
    {
        DialogueLoader.OnCallDialogueAction -= ChangeHealthBarValue;
    }

    private void Start()
    {
     ChangeHealthBarValue();   
    }

    private void ChangeHealthBarValue(ActionTypes type, int amount)
    {
        if (type == ActionTypes.Health)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, MAKS_HEALTH_VALUE);
            
            ChangeHealthBarValue();
        }
    }

    private void ChangeHealthBarValue()
    {
        float sliderValue = (float)currentHealth / MAKS_HEALTH_VALUE;
        healthBar.fillAmount = sliderValue;
    }
}
