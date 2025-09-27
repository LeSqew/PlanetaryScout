using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class HealthController : MonoBehaviour
{
    [SerializeField] private HpSetttings hpSetttings;
    public event Action<int> TakeDamage;
    public event Action<int> Heal;
    public HealthModel Model;

    [SerializeField] private InputActionReference takeDamage;
    [SerializeField] private InputActionReference heal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Model = new HealthModel(hpSetttings);
        TakeDamage += Model.TakeDamage;
        Heal += Model.Heal;
    }

    // Update is called once per frame
    void Update()
    {
        if (takeDamage.action.WasPressedThisFrame())
        {
            TakeDamage?.Invoke(10);
            PrintHp();
        }
        if (heal.action.WasPressedThisFrame())
        {
            Heal?.Invoke(10);
            PrintHp();
        }
    }
    public void PrintHp()
    {
        Debug.Log($"текущее hp: {Model.currentHealth}");
    }
}
