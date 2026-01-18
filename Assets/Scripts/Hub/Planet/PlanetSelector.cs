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
    [SerializeField] private Image planetPreviewImage; // Если хочешь картинку планеты

    [Header("Данные")]
    [SerializeField] private PlanetData[] allPlanets;
    [SerializeField] private InputActionAsset inputActions;

    private int _currentIndex = 0;
    private InputActionMap _playerMap;

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
        
            // Включаем или выключаем курсор в зависимости от того, в триггере мы или нет
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
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
        
        // Здесь же можно проверять ранг, как мы делали раньше
    }

    public void Launch()
    {
        _playerMap.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(allPlanets[_currentIndex].sceneName);
    }
}