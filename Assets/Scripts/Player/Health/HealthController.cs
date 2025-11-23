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
        healthBarView = new HealthBarView();
        TakeDamage += Model.TakeDamage;
        Heal += Model.Heal;
        OnDeath += Model.OnDeath;
        CheckHeal += Model.CheckHeal;
    }

    // Update is called once per frame
    
    
    public void ApplyDamage(int amount)
    {
        Model.TakeDamage(amount);
        Model.OnDeath(); // проверяет смерть
        healthBarView.UpdateHealthBar(Model.currentHealth, Model.maxHealth);
    }
    
    

}
