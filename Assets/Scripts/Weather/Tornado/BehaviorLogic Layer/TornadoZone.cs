using UnityEngine;

/// <summary>
/// Вспомогательный скрипт, прикрепленный к дочерним коллайдерам-зонам (Ядро/Внешняя). 
/// Фильтрует события столкновений и передает их основному скрипту TornadoEffect,
/// указывая, в какую именно зону вошел объект.
/// </summary>
public class TornadoZone : MonoBehaviour
{
    public enum ZoneType { Core, Outer }
    public ZoneType type;
    
    private TornadoEffect parentEffect;

    /// <summary>
    /// Находит и устанавливает ссылку на родительский скрипт TornadoEffect.
    /// </summary>
    void Start()
    {
        parentEffect = GetComponentInParent<TornadoEffect>(); 
        if (parentEffect == null)
        {
            Debug.LogError("TornadoZone не может найти TornadoEffect на родительском объекте!");
            enabled = false;
        }
    }

    /// <summary>
    /// Срабатывает при входе другого коллайдера в эту зону. Передает событие родителю.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (parentEffect != null)
        {
            parentEffect.ProcessEnter(other, type);
        }
    }
    
    /// <summary>
    /// Срабатывает при выходе другого коллайдера из этой зоны. Передает событие родителю.
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (parentEffect != null)
        {
            parentEffect.ProcessExit(other, type);
        }
    }
}