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

    [Header("Источник")]
    public Faction faction = Faction.None;

    [Header("Награда")]
    public int rewardMoney = 50;
    public int rewardResearchPoints = 20;
}