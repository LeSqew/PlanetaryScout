using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image background;

    public void UpdateSlot(InventoryItem item)
    {
        if (item != null && item.toolData != null)
        {
            icon.sprite = item.toolData.icon;
            icon.enabled = true;
            label.text = item.toolData.toolName;
        }
        else
        {
            icon.enabled = false;
            label.text = "";
        }
    }

    public void SetSelected(bool selected)
    {
        if (background != null)
        {
            // Включаем или выключаем подсветку
            background.enabled = selected;

            // Отладка: показываем, что делаем
            if (selected)
            {
                Debug.Log("Включаем подсветку для слота: " + gameObject.name);
            }
            else
            {
                Debug.Log("Выключаем подсветку для слота: " + gameObject.name);
            }
        }
    }
}
