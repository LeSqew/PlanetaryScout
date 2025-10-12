using UnityEngine;


public class TornadoZone : MonoBehaviour
{
    // Enum для указания типа зоны
    public enum ZoneType { Core, Outer }
    public ZoneType type;

    // Ссылка на основной скрипт эффекта на родительском объекте
    private TornadoEffect parentEffect;

    void Start()
    {
        // Находим родительский скрипт один раз при старте
        parentEffect = GetComponentInParent<TornadoEffect>(); 
        if (parentEffect == null)
        {
            Debug.LogError("TornadoZone не может найти TornadoEffect на родительском объекте!");
            enabled = false;
        }
    }
    
    // Этот метод вызывается, когда что-то ВХОДИТ в коллайдер зоны
    void OnTriggerEnter(Collider other)
    {
        // Передаем обнаруженный объект в главный скрипт TornadoEffect
        if (parentEffect != null)
        {
            parentEffect.ProcessEnter(other, type);
        }
    }

    // Этот метод вызывается, когда что-то ВЫХОДИТ из коллайдера зоны
    void OnTriggerExit(Collider other)
    {
        // Передаем обнаруженный объект в главный скрипт TornadoEffect
        if (parentEffect != null)
        {
            parentEffect.ProcessExit(other, type);
        }
    }
}