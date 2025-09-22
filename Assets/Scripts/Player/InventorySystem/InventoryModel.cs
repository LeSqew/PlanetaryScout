using System;
using System.Collections.Generic;

[Serializable]
public class InventoryItem {
    public int id;
    public string itemName;
}


public class InventoryModel
{
    public event Action<int, InventoryItem> OnSlotChanged;
    private InventoryItem[] _slots;
    public int slotCount => _slots.Length;

    public InventoryModel(int slotCount) {
        _slots = new InventoryItem[slotCount];
    }

    public void SetItem(int index, InventoryItem item) {
        if (index < 0 || index >= _slots.Length) return;
        _slots[index] = item;
        OnSlotChanged?.Invoke(index, item);
    }

    public InventoryItem GetItem(int index)
    {
        if (index < 0 || index >= _slots.Length) return null;
        return _slots[index];
    }
}
