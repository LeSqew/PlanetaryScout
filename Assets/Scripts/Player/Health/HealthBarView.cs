using UnityEngine;

public class HealthBarView: IHealthView
{
    public void PrintHp(int currentHealth)
    {
        Debug.Log($"текущее hp: {currentHealth}");
    }
}
