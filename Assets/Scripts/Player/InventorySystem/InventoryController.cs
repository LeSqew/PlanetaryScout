using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private int slotCount = 5;
    [SerializeField] private InventoryView[] slotViews;

    private InventoryModel _model;
    private int _currentSlotIndex = 0;

    private void Awake()
    {
        _model = new InventoryModel(slotCount);
        _model.OnSlotChanged += UpdateSlotView;
    }

    private void UpdateSlotView(int index, InventoryItem item)
    {
        if (index < slotViews.Length)
            slotViews[index].UpdateSlot(item);
    }

    public void FillSlot(int index, InventoryItem item)
    {
        _model.SetItem(index, item);
    }

    public void OnSelectSlot(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        var value = ctx.ReadValue<float>();

        var index = Mathf.RoundToInt(value) - 1;
        if (index < 0 || index >= _model.slotCount) return;
        _currentSlotIndex = index;
        Debug.Log("Выбран слот: " + (_currentSlotIndex + 1));
    }
}
