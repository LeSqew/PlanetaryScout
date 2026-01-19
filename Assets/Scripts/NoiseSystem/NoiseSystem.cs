using System;
using UnityEngine;
using UnityEngine.Events;
using static TreeEditor.TreeEditorHelper;
namespace SoundSystem
{
    public enum NoiseType
    {
        Footstep,
        Rock
    }
    public struct NoiseEvent
    {
        public Vector3 Position;
        public float Radius;
        public GameObject Source;
        public NoiseType? Type;
    }
    public static class NoiseSystem
    {
        public static UnityEvent<NoiseEvent> OnNoise = new();

        public static void MakeNoise(
            Vector3 position,
            float radius,
            GameObject source,
            NoiseType type)
        {
            OnNoise?.Invoke(new NoiseEvent
            {
                Position = position,
                Radius = radius,
                Source = source,
                Type = type
            });
        }
    }
}
