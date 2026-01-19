using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Add1", story: "[Int] increment", category: "Action", id: "e179f9ed256da00e9914810e66d48521")]
public partial class Add1Action : Action
{
    [SerializeReference] public BlackboardVariable<int> Int;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Int.Value += 1;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

