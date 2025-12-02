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
        if (isPaused)
        {
            ResumeGame();
        }
        else if (!MinigameManager.IsInMinigame && !MissionReportUI.IsDeathScreenActive)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        playerMap.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        playerMap.Enable();
        educationPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
}