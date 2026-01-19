using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HubController : MonoBehaviour
{
    [SerializeField] private List<PlanetData> availablePlanets;
    [SerializeField] private Transform hologramAnchor; // Точка, где крутится планета
    [SerializeField] private TextMeshProUGUI planetInfoText;
    [SerializeField] private Button startMissionButton;

    private int _currentIndex = 0;

    void Start()
    {
        UpdatePlanetDisplay();
    }

    public void NextPlanet()
    {
        _currentIndex = (_currentIndex + 1) % availablePlanets.Count;
        UpdatePlanetDisplay();
    }

    private void UpdatePlanetDisplay()
    {
        PlanetData data = availablePlanets[_currentIndex];
        
        // 1. Обновляем текст
        planetInfoText.text = $"{data.planetName}\nБиом: {data.biome}";

        // 2. Проверяем доступность по рангу
        bool isUnlocked = RankManager.Instance.CurrentRank >= data.requiredRank;
        
        startMissionButton.interactable = isUnlocked;
        
        if (!isUnlocked)
        {
            planetInfoText.text += $"\n<color=red>Требуется ранг: {data.requiredRank}</color>";
        }
        
        // 3. (Опционально) Меняем 3D модель планеты на столе
    }

    public void LaunchMission()
    {
        PlanetData data = availablePlanets[_currentIndex];
        // Загружаем сцену планеты
        UnityEngine.SceneManagement.SceneManager.LoadScene(data.sceneName);
    }
}