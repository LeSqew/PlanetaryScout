using UnityEngine;

public class InventorySetup : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;

    [Header("Инструменты")]
    [SerializeField] private ToolData[] defaultTools;

    private void Start()
    {
        if (inventoryController == null)
        {
            Debug.LogError("InventoryController не назначен!");
            return;
        }

        for (int i = 0; i < defaultTools.Length && i < inventoryController.slotCount; i++)
        {
            if (defaultTools[i] != null)
            {
                inventoryController.FillSlot(i, defaultTools[i]);
            }
        }
    }
}