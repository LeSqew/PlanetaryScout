using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BestiaryButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    private Action onSelected;

    public void Setup(string name, Action onClickAction)
    {
        titleText.text = name;
        onSelected = onClickAction;
        GetComponent<Button>().onClick.AddListener(() => onSelected?.Invoke());
    }
}