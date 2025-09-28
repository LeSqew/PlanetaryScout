using System;
using UnityEngine;

public class HealthModel
{
    public int currentHealth;
    public HealthModel(HpSetttings setttings)
    {
        currentHealth = setttings.MaxHP;
    }
    public void TakeDamage(int hit)
    {
        currentHealth = currentHealth - hit;
        //This was my first idea, but idk how good it is
        //currentHealth = Mathf.Max(0, currentHealth);
    }
    public void Heal(int heal)
    {
        currentHealth = currentHealth + heal;
        //This was my first idea, but idk how good it is
        //currentHealth = Mathf.Min(100, currentHealth);
    }
    public void OnDeath()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Death");
            currentHealth = 0;
        }
    }
    public void CheckHeal(int MaxHp, int currentheal) 
    {
        if (currentHealth >= MaxHp)
        {
            currentHealth = MaxHp;
        }
    }
}
