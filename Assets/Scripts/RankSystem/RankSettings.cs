using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlanetaryScout/Rank Settings")]
public class RankSettings : ScriptableObject
{
    public List<RankLevel> levels;
}

[Serializable]
public class RankLevel
{
    public string rankName;
    public int requiredXP; // XP, нужное ДЛЯ достижения этого ранга
    public int maxAllowedRarity; // Какую редкость открывает этот ранг
}