using UnityEngine;

public static class TornadoEvents
{
    public class PlayerCaughtEventArgs
    {
        public Vector3 TornadoPosition { get; }
        
        public PlayerCaughtEventArgs(Vector3 tornadoPosition)
        {
            TornadoPosition = tornadoPosition;
        }
    }

    public class PlayerReleasedEventArgs
    {
        public Vector3 TornadoPosition { get; }
        
        public PlayerReleasedEventArgs(Vector3 tornadoPosition)
        {
            TornadoPosition = tornadoPosition;
        }
    }

    public class PlayerThrownEventArgs
    {
        public Vector3 TornadoPosition { get; }
        public Vector3 ThrowDirection { get; }
        
        public PlayerThrownEventArgs(Vector3 tornadoPosition, Vector3 throwDirection)
        {
            TornadoPosition = tornadoPosition;
            ThrowDirection = throwDirection;
        }
    }

    public class MovedEventArgs
    {
        public Vector3 NewPosition { get; }
        
        public MovedEventArgs(Vector3 newPosition)
        {
            NewPosition = newPosition;
        }
    }

    public class TargetChangedEventArgs
    {
        public Vector3 NewTarget { get; }
        
        public TargetChangedEventArgs(Vector3 newTarget)
        {
            NewTarget = newTarget;
        }
    }
}