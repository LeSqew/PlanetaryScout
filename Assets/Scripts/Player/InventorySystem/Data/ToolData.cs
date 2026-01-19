using UnityEngine;

[CreateAssetMenu(fileName = "NewTool", menuName = "Inventory/Tool Data")]
public class ToolData : ScriptableObject
{
    [Header("Информация об инструменте")]
    public string toolName;
    public Sprite icon;              // Иконка для UI
    public GameObject modelPrefab;   // 3D-модель инструмента
    public DataCategory[] compatibleTypes; // Тип объекта, с которым можно взаимодействовать
    [Header("Мини-игра")]
    public GameObject minigamePrefab;
    [Header("Поведение при провале")]
    public bool destroyObjectOnFailure = false;
}