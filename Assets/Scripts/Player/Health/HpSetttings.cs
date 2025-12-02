using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Health
{
    [CreateAssetMenu(fileName = "HpSetttings", menuName = "Scriptable Objects/HpSetttings")]
    public class HpSetttings : ScriptableObject
    {
        [Header("Health")]
        [Min(0)]
        public int maxHp = 100;
    }
}
