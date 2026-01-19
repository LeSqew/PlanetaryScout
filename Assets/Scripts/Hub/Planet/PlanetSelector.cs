using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlanetSelector : MonoBehaviour
{
    [Header("UI элементы (Screen Space)")]
    [SerializeField] private GameObject missionPanel; 
    [SerializeField] private TextMeshProUGUI planetNameText;
    [SerializeField] private TextMeshProUGUI planetInfoText;
    
    [Header("Настройки голограммы")]
    [SerializeField] private Transform spawnPoint; // Пустой объект над столом
    [SerializeField] private float rotationSpeed = 20f;

    [Header("Данные")]
    [SerializeField] private PlanetData[] allPlanets;
    [SerializeField] private InputActionAsset inputActions;

    private int _currentIndex = 0;
    private InputActionMap _playerMap;
    private GameObject _currentModel;

    void Awake()
    {
        _playerMap = inputActions.FindActionMap("Player");
        missionPanel.SetActive(false);
    }

    // Тот самый метод, который вызовет HubInteractionHandler
    public void ShowPanel(bool isVisible)
    {
        if (missionPanel != null)
        {
            missionPanel.SetActive(isVisible);
            if (isVisible) UpdateUI();
            else 
            {
                // Скрываем планету, когда игрок отошел от стола
                if (_currentModel != null) Destroy(_currentModel);
            }
            
            // Включаем или выключаем курсор в зависимости от того, в триггере мы или нет
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
    
    void Update()
    {
        // Красивое вращение голограммы
        if (_currentModel != null)
        {
            _currentModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
    
    public void NextPlanet()
    {
        _currentIndex = (_currentIndex + 1) % allPlanets.Length;
        UpdateUI();
    }

    public void PreviousPlanet()
    {
        _currentIndex--;
        if (_currentIndex < 0) _currentIndex = allPlanets.Length - 1;
        UpdateUI();
    }

    private void UpdateUI()
    {
        PlanetData data = allPlanets[_currentIndex];
        planetNameText.text = data.planetName;
        planetInfoText.text = $"Биом: {data.biome}\n{data.description}";
        
        ChangePlanetModel(data.planetModelPrefab);
    }
    
    private void ChangePlanetModel(GameObject newPrefab)
    {
        // 1. Удаляем предыдущую планету, если она есть
        if (_currentModel != null)
        {
            Destroy(_currentModel);
        }

        // 2. Спавним новый префаб из PlanetData
        if (newPrefab != null)
        {
            _currentModel = Instantiate(newPrefab, spawnPoint.position, spawnPoint.rotation);
            _currentModel.transform.SetParent(spawnPoint); // Чтобы планета двигалась вместе со столом
        }
    }

    public void Launch()
    {
        _playerMap.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(allPlanets[_currentIndex].sceneName);
    }
}