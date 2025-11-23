using System;

public static class DataCollectionEvents
{
    public static event Action<ScanResult> OnDataCollected;
    public static event Action<DataCategory, int> OnScannableObjectDestroyed;

    public static void RaiseDataCollected(ScanResult result)
    {
        OnDataCollected?.Invoke(result);
    }
    
    public static void RaiseObjectDestroyed(DataCategory category, int remainingCount)
    {
        OnScannableObjectDestroyed?.Invoke(category, remainingCount);
    }
}