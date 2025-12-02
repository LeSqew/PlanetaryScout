using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Scriptable Objects/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    public float MouseSensitivity = 2.0f;
    public float MaxVerticalAngle = 90.0f;
    public float CameraHeight = 1.7f;
}