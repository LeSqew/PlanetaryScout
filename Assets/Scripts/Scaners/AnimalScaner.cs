using UnityEngine;
using UnityEngine.UI;

public class AnimalScanner : MonoBehaviour
{
    [Header("Scan Settings")]
    [SerializeField] private float scanTime = 2f;              // Время удержания ЛКМ для завершения сканирования
    [SerializeField] private Image scanProgressCircle;         // Круг вокруг курсора (UI Image)
    [SerializeField] private Camera mainCamera;                // Камера, из которой будет идти луч

    private float scanProgress = 0f;                           // Текущее заполнение круга (0–1)
    private GameObject currentTarget;                          // Объект, на который наведен курсор
    private bool isScanning = false;                           // Флаг состояния сканирования

    private void Start()
    {
        // Автоматически находим основную камеру, если не назначена
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Сбрасываем круг
        if (scanProgressCircle != null)
            scanProgressCircle.fillAmount = 0f;
    }

    private void Update()
    {
        HandleScanning();
        UpdateCirclePosition();
    }

    /// <summary>
    /// Основная логика: наведение, удержание, сканирование.
    /// </summary>
    private void HandleScanning()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Проверяем, попал ли луч в объект с тегом Animal
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Animal"))
        {
            currentTarget = hit.collider.gameObject;

            // Если ЛКМ удерживается — заполняем круг
            if (Input.GetMouseButton(0))
            {
                isScanning = true;
                scanProgress += Time.deltaTime / scanTime;
                scanProgress = Mathf.Clamp01(scanProgress);

                if (scanProgressCircle != null)
                    scanProgressCircle.fillAmount = scanProgress;

                // Скан завершён
                if (scanProgress >= 1f)
                {
                    Debug.Log($"✅ Скан завершён: {currentTarget.name}");
                    ResetScan();
                }
            }
            else
            {
                // ЛКМ отпущена — сбрасываем прогресс
                ResetScan();
            }
        }
        else
        {
            // Курсор не наведен на животное — сброс
            ResetScan();
        }
    }

    /// <summary>
    /// Обновляет позицию круга, чтобы он всегда был возле курсора.
    /// </summary>
    private void UpdateCirclePosition()
    {
        if (scanProgressCircle != null)
            scanProgressCircle.transform.position = Input.mousePosition;
    }

    /// <summary>
    /// Сбрасывает прогресс и состояние.
    /// </summary>
    private void ResetScan()
    {
        isScanning = false;
        scanProgress = 0f;

        if (scanProgressCircle != null)
            scanProgressCircle.fillAmount = 0f;
    }
}