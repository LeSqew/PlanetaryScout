using UnityEngine;

namespace Tornado
{
    public static class TornadoEvents
    {
        // Событие: Игрок пойман
        public class PlayerCaughtEventArgs
        {
            public Vector3 TornadoPosition { get; }
            public PlayerCaughtEventArgs(Vector3 position) => TornadoPosition = position;
        }

        // Событие: Игрок отпущен (без броска)
        public class PlayerReleasedEventArgs
        {
            public Vector3 TornadoPosition { get; }
            public PlayerReleasedEventArgs(Vector3 position) => TornadoPosition = position;
        }

        // Событие: Игрок выброшен силой
        public class PlayerThrownEventArgs
        {
            public Vector3 TornadoPosition { get; }
            public float ThrowForce { get; }

            public PlayerThrownEventArgs(Vector3 tornadoPosition, float throwForce)
            {
                TornadoPosition = tornadoPosition;
                ThrowForce = throwForce;
            }
        }

        // Событие: Торнадо сдвинулось (ТО САМОЕ ИСПРАВЛЕНИЕ)
        public class MovedEventArgs
        {
            public Vector3 NewPosition { get; }
            public MovedEventArgs(Vector3 newPosition) => NewPosition = newPosition;
        }

        // Событие: Смена цели движения
        public class TargetChangedEventArgs
        {
            public Vector3 NewTarget { get; }
            public TargetChangedEventArgs(Vector3 newTarget) => NewTarget = newTarget;
        }
    }
}