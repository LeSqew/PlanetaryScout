using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [Header("Параметры здоровья")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Эффекты")]
    public GameObject destructionPrefab; // Префаб для визуализации разрушения

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Этот метод вызывается компонентом TornadoEffect.cs
    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log($"Объект {gameObject.name} получил {amount} урона. Осталось: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 1. Визуальный эффект
        if (destructionPrefab != null)
        {
            Instantiate(destructionPrefab, transform.position, transform.rotation);
        }
        
        // 2. Логика удаления
        Destroy(gameObject);
        Debug.Log($"Объект {gameObject.name} полностью разрушен!");
    }
}
