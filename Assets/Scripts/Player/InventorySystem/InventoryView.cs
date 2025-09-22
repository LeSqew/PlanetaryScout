using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text label;

    public void UpdateSlot(InventoryItem item)
    {
        if (item != null)
        {
            icon.enabled = true;
            label.text = item.itemName;
        }
        else
        {
            icon.enabled = false;
            label.text = "";
        }
    }
}
