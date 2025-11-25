// InteractionHintUI.cs
using UnityEngine;
using TMPro;

public class InteractionHintUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        Hide();
    }

    public void Show(string text, Vector3 worldPosition)
    {
        hintText.text = text;
        hintText.gameObject.SetActive(true);

        // Конвертируем мировую позицию в экранную
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
        // Добавим немного выше объекта
        screenPos.y += 30f;
        transform.position = screenPos;
    }

    public void Hide()
    {
        hintText.gameObject.SetActive(false);
    }
}