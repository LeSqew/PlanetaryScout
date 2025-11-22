using UnityEngine;

[CreateAssetMenu(menuName = "PlanetaryScout/Quest Template")]
public class QuestTemplate : ScriptableObject
{
    [Header("Основное")]
    public string displayName;
    public string description;

    [Header("Контекст")]
    public Biome biome;
    public bool requiresWeather;
    public WeatherCondition weather = WeatherCondition.None;

    [Header("Цель")]
    public DataCategory goalCategory;
    [Range(1, 10)] public int minTargetCount = 3;
    [Range(1, 10)] public int maxTargetCount = 6;
    [Range(1, 4)] public int minRarity = 1;
    [Range(1, 4)] public int maxRarity = 2;

    [Header("Источник")]
    public Faction faction = Faction.None;

    [Header("Награда")]
    public int rewardMoney = 50;
    public int rewardResearchPoints = 20;
}