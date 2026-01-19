using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BestiaryManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<BestiaryEntry> allEntries; // Список всех ресурсов

    [Header("UI References")]
    [SerializeField] private Transform listContainer;       // Куда спавнить кнопки
    [SerializeField] private BestiaryButton buttonPrefab;    // Префаб кнопки

    [SerializeField] private TextMeshProUGUI nameDisplay;    // Текст названия справа
    [SerializeField] private TextMeshProUGUI descDisplay;    // Текст описания справа

    void Start()
    {
        PopulateList();
    }

    private void PopulateList()
    {
        // Очищаем список перед заполнением
        foreach (Transform child in listContainer) Destroy(child.gameObject);

        foreach (var entry in allEntries)
        {
            var btn = Instantiate(buttonPrefab, listContainer);
            btn.Setup(entry.objectName, () => ShowDetails(entry));
        }
    }

    public void ShowDetails(BestiaryEntry entry)
    {
        nameDisplay.text = entry.objectName;
        descDisplay.text = entry.description;

        // Здесь можно добавить логику отображения 3D модели через Render Texture
    }
}