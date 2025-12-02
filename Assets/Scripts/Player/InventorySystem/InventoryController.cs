using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.InventorySystem
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] public int slotCount = 5;
        [SerializeField] private InventoryView[] slotViews;
        [SerializeField] private InputActionReference switchToolAction;

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
            if (switchToolAction != null)
            {
                switchToolAction.action.performed += OnSelectSlot;
            }
        }
        private void OnDestroy()
        {
            if (switchToolAction != null)
            {
                switchToolAction.action.performed -= OnSelectSlot;
            }
        }
        private void UpdateSlotView(int index, InventoryItem item)
        {
            if (index < slotViews.Length)
                slotViews[index].UpdateSlot(item);
        }

        public void FillSlot(int index, ToolData toolData)
        {
            var item = new InventoryItem
            {
                id = toolData.GetInstanceID(),
                itemName = toolData.toolName,
                toolData = toolData
            };
            _model.SetItem(index, item);
            OnSlotChanged?.Invoke(index, item);
            UpdateSlotView(index, item);
        }

        public void OnSelectSlot(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            var value = ctx.ReadValue<float>();

            var index = Mathf.RoundToInt(value) - 1;
            Debug.Log("Value: " + value + ", Calculated index: " + index);

            if (index < 0 || index >= _model.slotCount)
            {
                Debug.Log("Index " + index + " выходит за границы. SlotCount: " + _model.slotCount);
                return;
            }
        

            // Сбрасываем подсветку старого слота
            slotViews[_currentSlotIndex].SetSelected(false);

            _currentSlotIndex = index;

            // Подсвечиваем новый слот
            slotViews[_currentSlotIndex].SetSelected(true);

            var item = _model.GetItem(_currentSlotIndex);
            OnSlotSelected?.Invoke(_currentSlotIndex, item);
        }

        public ToolData GetCurrentTool() => _model.GetItem(_currentSlotIndex)?.toolData;

        public bool CanInteractWith(DataCategory objType)
        {
            var tool = GetCurrentTool();
            if (tool == null) return false;

            // Проверяем, есть ли переданный тип в списке совместимых
            foreach (var type in tool.compatibleTypes)
            {
                if (type == objType) return true;
            }
            return false;
        }
    }
}
