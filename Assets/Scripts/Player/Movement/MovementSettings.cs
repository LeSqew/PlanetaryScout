using UnityEngine;

namespace Player.Movement
{
    [CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/MovementSettings")]
    public class MovementSettings : ScriptableObject
    {
        public float Acceleration;
        public float MaxSpeed;
        public float JumpForce;
        
    }
}
