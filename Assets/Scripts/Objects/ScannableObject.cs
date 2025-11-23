using UnityEngine;

/// <summary>
/// Универсальный компонент для любого сканируемого объекта.
/// Настраивается в инспекторе. Отправляет данные в систему заданий при вызове OnScanCompleted().
/// </summary>
[AddComponentMenu("Planetary Scout/ScannableObject")]
public class ScannableObject : MonoBehaviour
{
    [Header("Тип информации")]
    public DataCategory category = DataCategory.Flora;

    [Header("Редкость (1–4)")]
    [Range(1, 4)]
    public int rarity = 1;

    [Header("Отображаемое имя (опционально)")]
    public string displayName = "Неизвестный объект";

    /// <summary>
    /// Вызывается мини-игрой при успешном завершении сканирования.
    /// </summary>
    public void OnScanCompleted()
    {
        var result = new ScanResult
        {
            category = category,
            rarity = rarity,
            success = true
        };

        DataCollectionEvents.RaiseDataCollected(result);
    }
}
