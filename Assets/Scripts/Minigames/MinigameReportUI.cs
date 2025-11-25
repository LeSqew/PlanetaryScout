// MinigameReportUI.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MinigameReportUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failurePanel;
    [SerializeField] private Image screenOverlay;
    [SerializeField] private TextMeshProUGUI objectNameText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI errorMessageText;

    // Добавляем CanvasGroup для анимации прозрачности
    [Header("Animation")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float fadeOutDuration = 0.2f;

    private static MinigameReportUI _instance;
    public static MinigameReportUI Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowSuccessReport(ScannableObject target)
    {
        SetupReport();
        successPanel.SetActive(true);
        failurePanel.SetActive(false);
        
        screenOverlay.color = new Color(0.04f, 0.1f, 0.16f, 0.7f); 
        objectNameText.text = target.displayName;
        categoryText.text = $"Категория: {target.category}";
        rarityText.text = $"Редкость: {target.rarity}";
        
        StartCoroutine(ShowAndHide(2.5f));
    }

    public void ShowFailureReport(ScannableObject target, string reason)
    {
        SetupReport();
        successPanel.SetActive(false);
        failurePanel.SetActive(true);
        
        screenOverlay.color = new Color(0.16f, 0.04f, 0.04f, 0.7f);
        errorMessageText.text = reason;
        
        StartCoroutine(ShowAndHide(2.0f));
    }

    private void SetupReport()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f; // начинаем с прозрачности
    }

    private IEnumerator ShowAndHide(float showDuration)
    {
        // Плавное появление
        yield return FadeCanvasGroup(0f, 1f, fadeInDuration);
        
        // Ждём, пока отчёт виден
        yield return new WaitForSeconds(showDuration);
        
        // Плавное исчезновение
        yield return FadeCanvasGroup(1f, 0f, fadeOutDuration);
        
        gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, fadeCurve.Evaluate(elapsed / duration));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}