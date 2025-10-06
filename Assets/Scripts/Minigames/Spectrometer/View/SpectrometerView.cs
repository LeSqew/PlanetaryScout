using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpectrometerView : MonoBehaviour
{
    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;
    public Slider sliderUV;
    public Image accuracyFill;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI resultText;
    public RawImage spectrumImage;
    
    public event Action<float, float, float, float> OnSliderChanged;
    public event Action OnConfirmPressed;

    void Start()
    {
        sliderR.onValueChanged.AddListener(_ => NotifySliderChange());
        sliderG.onValueChanged.AddListener(_ => NotifySliderChange());
        sliderB.onValueChanged.AddListener(_ => NotifySliderChange());
        sliderUV.onValueChanged.AddListener(_ => NotifySliderChange());
        NotifySliderChange();
    }
    
    private void NotifySliderChange()
    {
        OnSliderChanged?.Invoke(
            sliderR.value,
            sliderG.value,
            sliderB.value,
            sliderUV.value
        );
    }
    
    public void SetAccuracy(float value)
    {
        if (accuracyFill != null)
            accuracyFill.fillAmount = Mathf.Clamp01(value);
        if (accuracyText != null)
            accuracyText.text = $"Точность: {Mathf.RoundToInt(value * 100)}%";
    }
    
    public void ShowResult(string result)
    {
        if (resultText != null)
            resultText.text = result;
    }

    public void RenderSpectrum(double[] measured, double[] target)
    {
        if (spectrumImage == null) return;
        spectrumImage.texture = SpectrumTextureGenerator.GenerateSpectrumTexture(measured, target);
    }

    public void OnConfirmButton() => OnConfirmPressed?.Invoke();
    
}
