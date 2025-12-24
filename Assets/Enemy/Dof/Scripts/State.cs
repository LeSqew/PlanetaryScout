using System;
using Unity.Behavior;

[BlackboardEnum]
public enum State
{
	Patrol,
	Check,
    Echo,
    Chase,
    Rush,
    Attack,
    Stunned,
    Chill
}
