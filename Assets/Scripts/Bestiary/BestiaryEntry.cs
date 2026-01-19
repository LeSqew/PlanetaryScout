using UnityEngine;

[CreateAssetMenu(fileName = "NewBestiaryEntry", menuName = "Game/Bestiary Entry")]
public class BestiaryEntry : ScriptableObject
{
    public string objectName;      // Название
    [TextArea(3, 10)]
    public string description;     // Описание
    public Sprite icon;            // Иконка (если нужна)
    public GameObject model3D;     // Ссылка на префаб для 3D-отображения в слоте
}