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
    }
    public void Heal(int heal)
    {
        currentHealth = currentHealth + heal;
    }
}
