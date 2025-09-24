using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private int slotCount = 5;
    [SerializeField] private InventoryView[] slotViews;

    private InventoryModel _model;
    private int _currentSlotIndex = 0;

    public event Action<int, InventoryItem> OnSlotChanged;
    public event Action<int, InventoryItem> OnSlotSelected;
    private void Awake()
    {
        _model = new InventoryModel(slotCount);
        for (int i = 0; i < slotCount; i++)
        {
            UpdateSlotView(i, _model.GetItem(i));
        }
    }

    private void UpdateSlotView(int index, InventoryItem item)
    {
        if (index < slotViews.Length)
            slotViews[index].UpdateSlot(item);
    }

    public void FillSlot(int index, InventoryItem item)
    {
        _model.SetItem(index, item);
        OnSlotChanged?.Invoke(index, item);
        UpdateSlotView(index, item);
    }

    public void OnSelectSlot(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        var value = ctx.ReadValue<float>();
        var index = Mathf.RoundToInt(value) - 1;
        if (index < 0 || index >= _model.slotCount) return;
        _currentSlotIndex = index;
        var item = _model.GetItem(_currentSlotIndex);
        OnSlotSelected?.Invoke(_currentSlotIndex, item);
        Debug.Log($"Выбран слот: {_currentSlotIndex + 1}, предмет: {item?.itemName ?? "пусто"}");
    }
}
