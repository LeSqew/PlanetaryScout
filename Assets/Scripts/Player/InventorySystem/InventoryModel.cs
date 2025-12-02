using System;
using System.Collections.Generic;

[Serializable]
public class InventoryItem {
    public int id;
    public string itemName; // можно удалить, если не нужен
    public ToolData toolData;
}


public class InventoryModel
{
    private InventoryItem[] _slots;
    public int slotCount => _slots.Length;

    public InventoryModel(int slotCount) {
        _slots = new InventoryItem[slotCount];
    }

    public void SetItem(int index, InventoryItem item) {
        if (index < 0 || index >= _slots.Length) return;
        _slots[index] = item;
    }

    public InventoryItem GetItem(int index)
    {
        if (index < 0 || index >= _slots.Length) return null;
        return _slots[index];
    }
}
