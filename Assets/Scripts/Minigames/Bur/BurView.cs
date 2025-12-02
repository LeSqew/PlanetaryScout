using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BurView
{
    // UI references (provided from controller)
    private readonly RectTransform containerRect;
    private readonly RectTransform movingPoint;
    private readonly RectTransform greenZone;
    private readonly Image pointImage;
    private readonly GameObject winScreen;
    private readonly GameObject loseScreen;
    private readonly TMP_Text timerText;
    private readonly TMP_Text statusText;

    public BurView(
        RectTransform containerRect,
        RectTransform movingPoint,
        RectTransform greenZone,
        Image pointImage,
        GameObject winScreen,
        GameObject loseScreen,
        TMP_Text timerText,
        TMP_Text statusText)
    {
        this.containerRect = containerRect;
        this.movingPoint = movingPoint;
        this.greenZone = greenZone;
        this.pointImage = pointImage;
        this.winScreen = winScreen;
        this.loseScreen = loseScreen;
        this.timerText = timerText;
        this.statusText = statusText;
    }

    // Update the visible position of the moving point (normalized 0..1)
    public void SetPointNormalized(float normalized)
    {
        float containerWidth = containerRect.rect.width;
        float pointX = (normalized - 0.5f) * containerWidth;
        movingPoint.anchoredPosition = new Vector2(pointX, movingPoint.anchoredPosition.y);
    }

    public void SetGreenZoneWidth(float width)
    {
        greenZone.sizeDelta = new Vector2(width, greenZone.sizeDelta.y);
    }

    public void SetPointColor(bool isInGreen)
    {
        if (pointImage != null) pointImage.color = isInGreen ? Color.green : Color.red;
    }

    public void ShowWinScreen(bool show)
    {
        if (winScreen != null) winScreen.SetActive(show);
    }

    public void ShowLoseScreen(bool show)
    {
        if (loseScreen != null) loseScreen.SetActive(show);
    }

    public void SetTimerText(string text, bool green)
    {
        if (timerText != null)
        {
            timerText.text = text;
            timerText.color = green ? Color.green : Color.red;
        }
    }

    public void SetStatusText(string text)
    {
        if (statusText != null) statusText.text = text;
    }

    // Helper: returns container width (for controller)
    public float ContainerWidth => containerRect.rect.width;

    // Helper: current point anchored x (for controller calculations)
    public float CurrentPointAnchoredX => movingPoint.anchoredPosition.x;
}