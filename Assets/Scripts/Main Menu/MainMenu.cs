using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button educationButton;
    [SerializeField] private Button exitButton;

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject educationPanel;

    [Header("Education Panel Elements")]
    [SerializeField] private Button backButton;

    private void Start()
    {
        SetupButtonListeners();
        educationPanel.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        educationButton.onClick.AddListener(OnEducationButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    //Запуск следующей сцены по их порядку в билде
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnEducationButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        educationPanel.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        mainMenuPanel.SetActive(true);
        educationPanel.SetActive(false);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (playButton != null) playButton.onClick.RemoveListener(OnPlayButtonClicked);
        if (educationButton != null) educationButton.onClick.RemoveListener(OnEducationButtonClicked);
        if (exitButton != null) exitButton.onClick.RemoveListener(OnExitButtonClicked);
        if (backButton != null) backButton.onClick.RemoveListener(OnBackButtonClicked);
    }
}
