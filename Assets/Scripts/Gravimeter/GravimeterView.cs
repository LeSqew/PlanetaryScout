using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GravimeterView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI parametersText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private RawImage waveImage; // ← один RawImage для обеих кривых
    
    [Header("Кнопка подтверждения")]
    [SerializeField] private Button confirmButton;
    
    private Texture2D _waveTexture;

    private const int WAVE_TEXTURE_WIDTH = 256;
    private const int WAVE_TEXTURE_HEIGHT = 64;
    private const float MAX_AMPLITUDE = 20f;

    void Awake()
    {
        _waveTexture = CreateClearTexture();
        if (waveImage != null) waveImage.texture = _waveTexture;
    }
    
    private GravimeterController _controller;

    void Start()
    {
        // Получаем контроллер (если он на том же объекте или дочернем)
        _controller = GetComponent<GravimeterController>() 
                      ?? GetComponentInParent<GravimeterController>()
                      ?? FindObjectOfType<GravimeterController>();

        if (confirmButton != null && _controller != null)
        {
            confirmButton.onClick.AddListener(_controller.OnConfirmPressed);
        }
    }


    // --- Публичные методы для Controller ---
    public void SetWaveData(WaveParams target, WaveParams player, float remainingTime, float dataQuality)
    {
        UpdateParametersText(target, player);
        UpdateTimerText(remainingTime, dataQuality);
        UpdateQualityText(dataQuality);
        UpdateStatusText(false, remainingTime > 0);
    
        // Рисуем ОБЕ кривые на одной текстуре
        DrawCombinedWave(_waveTexture, target, player);
    
        if (waveImage != null) waveImage.texture = _waveTexture;
    }

    private void DrawCombinedWave(Texture2D texture, WaveParams target, WaveParams player)
    {
        Color[] pixels = new Color[WAVE_TEXTURE_WIDTH * WAVE_TEXTURE_HEIGHT];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;

        float timeRange = 2f * Mathf.PI;

        // 1. Рисуем ЦЕЛЕВУЮ кривую (красная) — сначала, чтобы игрок мог её "накрыть"
        for (int x = 0; x < WAVE_TEXTURE_WIDTH; x++)
        {
            float t = (x / (float)(WAVE_TEXTURE_WIDTH - 1)) * timeRange;
            float yValue = target.Amplitude * Mathf.Sin(target.Frequency * t + target.PhaseShift);
            float yNorm = Mathf.Clamp01((yValue / MAX_AMPLITUDE + 1f) * 0.5f);
            int yPixel = Mathf.FloorToInt(yNorm * (WAVE_TEXTURE_HEIGHT - 1));
            if (yPixel >= 0 && yPixel < WAVE_TEXTURE_HEIGHT)
                pixels[yPixel * WAVE_TEXTURE_WIDTH + x] = Color.red;
        }

        // 2. Рисуем ИГРОВУЮ кривую (голубая) — поверх целевой
        for (int x = 0; x < WAVE_TEXTURE_WIDTH; x++)
        {
            float t = (x / (float)(WAVE_TEXTURE_WIDTH - 1)) * timeRange;
            float yValue = player.Amplitude * Mathf.Sin(player.Frequency * t + player.PhaseShift);
            float yNorm = Mathf.Clamp01((yValue / MAX_AMPLITUDE + 1f) * 0.5f);
            int yPixel = Mathf.FloorToInt(yNorm * (WAVE_TEXTURE_HEIGHT - 1));
            if (yPixel >= 0 && yPixel < WAVE_TEXTURE_HEIGHT)
                pixels[yPixel * WAVE_TEXTURE_WIDTH + x] = Color.cyan;
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }
    
    public void ShowSuccess()
    {
        if (statusText)
        {
            statusText.text = "!!! УСПЕХ! АНОМАЛИЯ ЗАФИКСИРОВАНА !!!";
            statusText.color = Color.green;
        }
    }

    public void ShowFailure()
    {
        if (statusText)
        {
            statusText.text = "!!! СБОЙ! АНОМАЛИЯ ПРОПУЩЕНА. ДАННЫЕ ИСКАЖЕНЫ !!!";
            statusText.color = Color.magenta;
        }
    }

    // --- Внутренние методы ---
    private void UpdateParametersText(WaveParams target, WaveParams player)
    {
        if (parametersText == null) return;
        var sb = new StringBuilder();
        sb.AppendLine("--- ПАРАМЕТРЫ ВОЛН ---");
        sb.AppendLine($"ЦЕЛЬ: A:{target.Amplitude:F2} F:{target.Frequency:F2} P:{target.PhaseShift:F2}");
        sb.AppendLine($"ИГРОК: A:{player.Amplitude:F2} F:{player.Frequency:F2} P:{player.PhaseShift:F2}");
        parametersText.text = sb.ToString();
    }

    private void UpdateTimerText(float time, float quality)
    {
        if (timerText == null) return;
        timerText.text = $"ОСТАВШЕЕСЯ ВРЕМЯ: {time:F1} сек.";
        timerText.color = (time <= 5f) ? Color.yellow : Color.white;
    }

    private void UpdateQualityText(float quality)
    {
        if (qualityText == null) return;
        qualityText.text = $"КАЧЕСТВО ДАННЫХ: {quality:P1}";
        qualityText.color = Color.Lerp(Color.red, Color.green, quality);
    }

    private void UpdateStatusText(bool matched, bool timeLeft)
    {
        if (statusText == null) return;
        if (!matched && timeLeft)
        {
            statusText.text = " ИДЕТ СОВМЕЩЕНИЕ...";
            statusText.color = Color.red;
        }
    }

    private Texture2D CreateClearTexture()
    {
        var tex = new Texture2D(WAVE_TEXTURE_WIDTH, WAVE_TEXTURE_HEIGHT);
        var clear = new Color[WAVE_TEXTURE_WIDTH * WAVE_TEXTURE_HEIGHT];
        for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
        tex.SetPixels(clear);
        tex.Apply();
        return tex;
    }
}