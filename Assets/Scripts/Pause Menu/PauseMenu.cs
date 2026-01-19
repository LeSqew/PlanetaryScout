using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button educationButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject educationPanel;

    [Header("Education Panel Elements")]
    [SerializeField] private Button backButton;

    [Header("Action Map")]
    [SerializeField] private InputActionAsset inputActionAsset;

    public static bool IsPaused { get; private set; }

    private InputActionMap UIActionMap;
    private InputActionMap playerMap;
    private InputAction pauseAction;
    private bool isPaused = false;

    private void Awake()
    {
        UIActionMap = inputActionAsset.FindActionMap("UI", true);
        playerMap = inputActionAsset.FindActionMap("Player", true);
        pauseAction = UIActionMap.FindAction("Pause");
    }

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePerformed;
        pauseAction.Disable();
    }

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        educationPanel.SetActive(false);
        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        continueButton.onClick.AddListener(ResumeGame);
        educationButton.onClick.AddListener(OnEducationButtonClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }
    

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        // Условие: Не открывать паузу, если открыт бестиарий, миниигра или экран смерти
        else if (!MinigameManager.IsInMinigame && 
                 !MissionReportUI.IsDeathScreenActive && 
                 !BestiaryManager.IsBestiaryOpen) 
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        IsPaused = true; // Устанавливаем флаг
        pauseMenuPanel.SetActive(true);
        playerMap.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f; // Обычно пауза останавливает время
    }

    public void ResumeGame()
    {
        IsPaused = false; // Снимаем флаг
        pauseMenuPanel.SetActive(false);
        playerMap.Enable();
        educationPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void OnEducationButtonClicked()
    {
        pauseMenuPanel.SetActive(false);
        educationPanel.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        pauseMenuPanel.SetActive(true);
        educationPanel.SetActive(false);
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        if (continueButton != null) continueButton.onClick.RemoveListener(ResumeGame);
        if (educationButton != null) educationButton.onClick.RemoveListener(OnEducationButtonClicked);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        if (backButton != null) backButton.onClick.RemoveListener(OnBackButtonClicked);
    }
    public void ReturnToHub()
    {
        Time.timeScale = 1f; // Важно! В отчете время обычно остановлено
        isPaused = false;
        playerMap.Enable();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hub");
    }
}