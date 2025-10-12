using UnityEngine;

/// <summary>
/// Компонент, позволяющий объекту (например, зданию) получать урон и быть разрушенным.
/// </summary>
public class DestructibleObject : MonoBehaviour
{
    [Header("Параметры здоровья")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Эффекты")]
    public GameObject destructionPrefab; 

    void Start()
    {
        currentHealth = maxHealth;
    }
    
    /// <summary>
    /// Наносит урон объекту. Вызывается компонентом TornadoEffect.cs.
    /// </summary>
    /// <param name="amount">Количество урона для вычитания из текущего здоровья.</param>
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
    
    /// <summary>
    /// Обрабатывает последовательность уничтожения: создает префаб обломков и уничтожает основной объект.
    /// </summary>
    private void Die()
    {
        if (destructionPrefab != null)
        {
            Instantiate(destructionPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
        Debug.Log($"Объект {gameObject.name} полностью разрушен!");
    }
}
