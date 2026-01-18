using UnityEngine;

[CreateAssetMenu(menuName = "PlanetaryScout/Planet Data")]
public class PlanetData : ScriptableObject
{
    [Header("Общая информация")]
    public string planetName;
    [TextArea] public string description;
    public Biome biome;
    public string sceneName; // Имя сцены, которую загрузим

    [Header("Требования")]
    public int requiredRank; // Какой ранг нужен для доступа

    [Header("Визуализация в Хабе")]
    public GameObject planetModelPrefab; // Префаб вращающейся планеты для стола
    public Color hologramColor = Color.cyan;
}