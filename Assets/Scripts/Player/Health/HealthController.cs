using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class HealthController : MonoBehaviour
{
    [SerializeField] private HpSetttings hpSetttings;
    public event Action<int> TakeDamage;
    public event Action<int> Heal;
    public event Action OnDeath;
    public event Action<int,int> CheckHeal;
    public HealthModel Model;
    public HealthBarView healthBarView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Model = new HealthModel(hpSetttings);
        TakeDamage += Model.TakeDamage;
        Heal += Model.Heal;
        OnDeath += Model.OnDeath;
        CheckHeal += Model.CheckHeal;
    }

    // Update is called once per frame
    
    
    public void ApplyDamage(int amount)
    {
        Model.TakeDamage(amount);
        Model.OnDeath(); // РїСЂРѕРІРµСЂСЏРµС‚ СЃРјРµСЂС‚СЊ
        healthBarView.UpdateHealthBar(Model.currentHealth, Model.maxHealth);
    }
    
    

    public void ApplyDamage(int damage)
    {
        TakeDamage?.Invoke(damage);
        // Проверяем смерть после получения урона
        if (Model.currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        healthBarView.PrintHp(Model.currentHealth);
    }

}
