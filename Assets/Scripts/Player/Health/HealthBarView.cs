using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBarView : MonoBehaviour 
{
    [SerializeField]
    private Image fillImage;
    
    public void PrintHp(int currentHealth)
    {
        Debug.Log($"Текущее здоровье: {currentHealth}");
    }
    
    public void UpdateHealthBar(int currentHealth, int maxHealth) 
    {
        if (fillImage == null)
        {
            Debug.LogError("Заполнение изображения не назначено в инспекторе HealthBarView.");
            return;
        }

        float fillAmount = (float)currentHealth / maxHealth;
        
        fillImage.fillAmount = fillAmount;

        PrintHp(currentHealth); 
    }
}