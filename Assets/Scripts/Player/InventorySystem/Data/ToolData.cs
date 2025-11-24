using UnityEngine;

[CreateAssetMenu(fileName = "NewTool", menuName = "Inventory/Tool Data")]
public class ToolData : ScriptableObject
{
    [Header("Информация об инструменте")]
    public string toolName;
    public Sprite icon;              // Иконка для UI
    public GameObject modelPrefab;   // 3D-модель инструмента (например, для отображения в руке или в инвентаре)
    public ObjectType[] compatibleTypes; // Тип объекта, с которым можно взаимодействовать
}

public enum ObjectType
{
    Flora,
    Fauna,
    Mineral,
    WeatherAnomaly
}