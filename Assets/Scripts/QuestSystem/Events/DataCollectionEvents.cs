using System;

public static class DataCollectionEvents
{
    public static event Action<ScanResult> OnDataCollected;

    public static void RaiseDataCollected(ScanResult result)
    {
        OnDataCollected?.Invoke(result);
    }
}