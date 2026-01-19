using UnityEngine;

[CreateAssetMenu(fileName = "NewBestiaryEntry", menuName = "Game/Bestiary Entry")]
public class BestiaryEntry : ScriptableObject
{
    public string objectName;      // ��������
    [TextArea(3, 10)]
    public string description;     // ��������

    public string rarity;             // �������������            // ������ (���� �����)
    public GameObject model3D;     // ������ �� ������ ��� 3D-����������� � �����
}