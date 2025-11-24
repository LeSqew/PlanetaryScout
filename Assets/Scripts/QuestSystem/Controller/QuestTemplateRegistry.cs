using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "PlanetaryScout/Quest Template Registry")]
public class QuestTemplateRegistry : ScriptableObject
{
    public List<QuestTemplate> allTemplates = new();
}