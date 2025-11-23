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

    [SerializeField] private InputActionReference takeDamage;
    [SerializeField] private InputActionReference heal;
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
    void Update()
    {
        if (takeDamage.action.WasPressedThisFrame())
        {
            TakeDamage?.Invoke(10);
            OnDeath?.Invoke();
            healthBarView.UpdateHealthBar(Model.currentHealth, Model.maxHealth);
        }
        if (heal.action.WasPressedThisFrame())
        {
            Heal?.Invoke(10);
            CheckHeal?.Invoke(hpSetttings.MaxHP, 10);
            healthBarView.UpdateHealthBar(Model.currentHealth, Model.maxHealth);
        }
    }

    public void ApplyDamage(int damage)
    {
        TakeDamage?.Invoke(damage);
        // ѕровер€ем смерть после получени€ урона
        if (Model.currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        healthBarView.PrintHp(Model.currentHealth);
    }

}
