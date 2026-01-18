using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlanetSelector : MonoBehaviour
{
    [Header("Список всех планет")]
    [SerializeField] private PlanetData[] allPlanets;

    [Header("Ссылки на UI стола")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button launchButton;
    [SerializeField] private TextMeshProUGUI rankWarningText;

    [Header("Точка визуализации")]
    [SerializeField] private Transform displayPivot; // Сюда спавним модель
    [SerializeField] private float rotationSpeed = 20f;

    private int _currentIndex = 0;
    private GameObject _currentModel;

    void Start()
    {
        UpdateDisplay();
    }

    void Update()
    {
        // Просто вращаем модель планеты для красоты
        if (_currentModel != null)
        {
            _currentModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public void NextPlanet()
    {
        _currentIndex = (_currentIndex + 1) % allPlanets.Length;
        UpdateDisplay();
    }

    public void PreviousPlanet()
    {
        _currentIndex--;
        if (_currentIndex < 0) _currentIndex = allPlanets.Length - 1;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        PlanetData data = allPlanets[_currentIndex];

        // 1. Обновляем текстовую информацию
        nameText.text = data.planetName;
        infoText.text = $"Биом: {data.biome}\n{data.description}";

        // 2. Проверяем ранг через RankManager
        int playerRank = RankManager.Instance != null ? RankManager.Instance.CurrentRank : 0;
        bool isUnlocked = playerRank >= data.requiredRank;

        launchButton.interactable = isUnlocked;
        rankWarningText.gameObject.SetActive(!isUnlocked);
        if (!isUnlocked)
        {
            rankWarningText.text = $"ТРЕБУЕТСЯ РАНГ: {data.requiredRank}";
        }

        // 3. Обновляем 3D модель
        if (_currentModel != null) Destroy(_currentModel);
        if (data.planetModelPrefab != null)
        {
            _currentModel = Instantiate(data.planetModelPrefab, displayPivot);
            // Устанавливаем слои или материалы голограммы, если нужно
        }
    }

    public void LaunchMission()
    {
        // Перед вылетом можно сбросить MissionStatus
        MissionStatus.Reset();
        SceneManager.LoadScene(allPlanets[_currentIndex].sceneName);
    }
}