using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class BurMiniGame : MonoBehaviour
{
    [Header("Game Settings")]
    //speed of point
    public float moveSpeed = 0.3f;
    public float driftStrength = 0.1f;
    public float driftChangeFrequency = 2f;
    public float greenZoneWidth = 200f;
    public float winTime = 10f;
    public float loseTime = 5f;

    [Header("UI References")]
    public RectTransform movingPoint;
    public RectTransform greenZone;
    public Image pointImage;
    public GameObject winScreen;
    public GameObject loseScreen;
    public TMP_Text timerText;
    public TMP_Text statusText;

    [Header("Input Actions")]
    public InputActionReference leftClickAction;
    public InputActionReference rightClickAction;

    // Приватные переменные
    private float currentPosition = 0.5f;
    private float currentDrift;
    private float nextDriftChange;
    private float timeInGreenZone;
    private float timeInRedZone;
    private bool isInGreenZone;
    private bool gameActive = true;
    private bool breakMessageShown = false;

    // Для Input System - будем проверять состояние каждый кадр
    private InputAction leftClick;
    private InputAction rightClick;

    void Start()
    {
        // Получаем ссылки на Input Actions
        leftClick = leftClickAction.action;
        rightClick = rightClickAction.action;

        // Включаем действия
        leftClick.Enable();
        rightClick.Enable();

        // Начальная настройка
        currentPosition = 0.5f;
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        UpdateVisuals();
        UpdateStatusText("Держите бур в зеленой зоне!");
    }

    void OnDestroy()
    {
        // Отключаем действия при уничтожении объекта
        if (leftClick != null) leftClick.Disable();
        if (rightClick != null) rightClick.Disable();
    }

    void Update()
    {
        if (!gameActive) return;

        HandleInput();
        ApplyDrift();
        UpdateVisuals();
        CheckZone();
        UpdateTimers();
        UpdateUI();
    }

    void HandleInput()
    {
        // Проверяем состояние кнопок каждый кадр
        bool leftClickHeld = leftClick.ReadValue<float>() > 0;
        bool rightClickHeld = rightClick.ReadValue<float>() > 0;

        // Если нажаты обе кнопки - не двигаемся
        if (leftClickHeld && rightClickHeld)
        {
            // Можно добавить какой-то эффект или оставить пустым
        }
        // Движение вправо при зажатой ЛКМ
        else if (leftClickHeld)
        {
            currentPosition += moveSpeed * Time.deltaTime;
        }
        // Движение влево при зажатой ПКМ
        else if (rightClickHeld)
        {
            currentPosition -= moveSpeed * Time.deltaTime;
        }

        // Ограничение значения от 0 до 1
        currentPosition = Mathf.Clamp01(currentPosition);
    }

    void ApplyDrift()
    {
        if (Time.time > nextDriftChange)
        {
            currentDrift = Random.Range(-1f, 1f) * driftStrength;
            nextDriftChange = Time.time + (1f / driftChangeFrequency);
        }

        currentPosition += currentDrift * Time.deltaTime;
        currentPosition = Mathf.Clamp01(currentPosition);
    }

    void UpdateVisuals()
    {
        float containerWidth = GetComponent<RectTransform>().rect.width;
        float pointX = (currentPosition - 0.5f) * containerWidth;
        movingPoint.anchoredPosition = new Vector2(pointX, 0);
        greenZone.sizeDelta = new Vector2(greenZoneWidth, greenZone.sizeDelta.y);
    }

    void CheckZone()
    {
        float zoneLeft = -greenZoneWidth / 2;
        float zoneRight = greenZoneWidth / 2;
        float pointX = movingPoint.anchoredPosition.x;

        bool wasInGreenZone = isInGreenZone;
        isInGreenZone = (pointX >= zoneLeft && pointX <= zoneRight);

        pointImage.color = isInGreenZone ? Color.green : Color.red;

        // Сбрасываем противоположный таймер при смене зоны
        if (isInGreenZone && !wasInGreenZone)
        {
            timeInRedZone = 0;
            breakMessageShown = false;
        }
        else if (!isInGreenZone && wasInGreenZone)
        {
            timeInGreenZone = 0;
        }
    }

    void UpdateTimers()
    {
        if (isInGreenZone)
        {
            timeInGreenZone += Time.deltaTime;
            timeInRedZone = 0;

            if (timeInGreenZone >= winTime)
            {
                WinGame();
            }
        }
        else
        {
            timeInRedZone += Time.deltaTime;

            if (timeInRedZone >= loseTime && !breakMessageShown)
            {
                LoseGame();
                breakMessageShown = true;
            }
        }
    }

    void UpdateUI()
    {
        // Обновляем текст таймера
        if (isInGreenZone)
        {
            float timeLeft = winTime - timeInGreenZone;
            timerText.text = $"До раскопки: {timeLeft:F1}с";
            timerText.color = Color.green;
        }
        else
        {
            float timeLeft = loseTime - timeInRedZone;
            timerText.text = $"До поломки: {timeLeft:F1}с";
            timerText.color = Color.red;
        }
    }

    void UpdateStatusText(string message)
    {
        statusText.text = message;
    }

    void WinGame()
    {
        gameActive = false;
        Debug.Log("Успешная раскопка! Бур достиг цели!");
        winScreen.SetActive(true);
        UpdateStatusText("УСПЕШНАЯ РАСКОПКА!");
        StartCoroutine(ResetGameAfterDelay(3f));
    }

    void LoseGame()
    {
        gameActive = false;
        Debug.Log("Бур сломался! Слишком долго в красной зоне.");
        loseScreen.SetActive(true);
        UpdateStatusText("БУР СЛОМАЛСЯ!");
        StartCoroutine(ResetGameAfterDelay(3f));
    }

    IEnumerator ResetGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetGame();
    }

    void ResetGame()
    {
        timeInGreenZone = 0;
        timeInRedZone = 0;
        currentPosition = 0.5f;
        breakMessageShown = false;
        gameActive = true;
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        UpdateVisuals();
        UpdateStatusText("Держите бур в зеленой зоне!");
    }
}