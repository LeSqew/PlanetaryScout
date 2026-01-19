using SoundSystem;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ListenNoise", story: "[Agent] listens noise with [NoiseMultiplier] and writes ladt sound position to [LastSound] and checks [isPlayer]", category: "Action", id: "2520fb63433833d65511662050efb9c8")]
public partial class ListenNoiseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> NoiseMultiplier;
    [SerializeReference] public BlackboardVariable<Vector3> LastSound;
    [SerializeReference] public BlackboardVariable<bool> IsPlayer;
    public bool HasHeardSound;
    protected override Status OnStart()
    {
        HasHeardSound = false;
        NoiseSystem.OnNoise.AddListener(OnNoiseHeard);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Debug.Log("Listening for noise...");
        if (HasHeardSound)
        {
            return Status.Success;
        }
        else return Status.Running;
    }

    protected override void OnEnd()
    {
        HasHeardSound = false;
        NoiseSystem.OnNoise.RemoveListener(OnNoiseHeard);
    }

    private bool HasLineOfSound(NoiseEvent noise)
    {
        var agentTransform = Agent.Value.transform;
        Vector3 dir = noise.Position - agentTransform.position;
        return !Physics.Raycast(
            agentTransform.position,
            dir.normalized,
            noise.Radius * NoiseMultiplier.Value
        );
    }

    private float GetPathLength(NavMeshPath path)
    {
        float length = 0f;
        for (int i = 1; i < path.corners.Length; i++)
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        return length;
    }

    private void OnNoiseHeard(NoiseEvent noise)
    {
        Debug.Log("Noise heard at position: " + noise.Position);
        // Check direct line of sound
        if (HasLineOfSound(noise))
        {
            ReactToNoise(noise);
            return;
        }

        // Check path distance
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(
            Agent.Value.transform.position,
            noise.Position,
            NavMesh.AllAreas,
            path
        );

        var distance = GetPathLength(path);
        if (distance <= noise.Radius * NoiseMultiplier.Value)
        {
            ReactToNoise(noise);
        }
    }
    private void ReactToNoise(NoiseEvent noise)
    {
        LastSound.Value = noise.Position;
        HasHeardSound = true;
        if (noise.Type == SoundSystem.NoiseType.Footstep)
        {
            IsPlayer.Value = true;
        }
        else
        {
            IsPlayer.Value = false;
        }
    }
}

