using UnityEngine;

public class TestStart : MonoBehaviour
{
    public QuestController questController;
    void Start()
    {

        questController.GenerateBaseQuests(Biome.Biosphere, WeatherCondition.None);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
