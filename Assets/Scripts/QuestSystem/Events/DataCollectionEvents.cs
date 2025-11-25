using System;
using UnityEngine;

public static class DataCollectionEvents
{
    public static event Action<ScanResult> OnDataCollected;
    
    // Убираем remainingCount — будем считать внутри QuestController
    public static event Action<DataCategory> OnScannableObjectDestroyed;

    public static void RaiseDataCollected(ScanResult result)
    {
        OnDataCollected?.Invoke(result);
    }
    
    public static void RaiseObjectDestroyed(DataCategory category)
    {
        OnScannableObjectDestroyed?.Invoke(category);
    }
}