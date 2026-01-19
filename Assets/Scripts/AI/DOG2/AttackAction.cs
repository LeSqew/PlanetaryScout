using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Player.Health;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "Agent attacks [Target] in [HitBox] with [Damage]", category: "Action", id: "90425734973bc5fd30b9a48b27d52736")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<SphereCollider> HitBox;
    [SerializeReference] public BlackboardVariable<int> Damage;
    private HealthController controller;

    protected override Status OnStart()
    {
        controller = Target.Value.GetComponent<HealthController>();
        if (!controller) return Status.Failure;
        if (HitBox == null) return Status.Failure;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var collapses = Physics.OverlapSphere(HitBox.Value.transform.position, HitBox.Value.radius);
        foreach (var collapser in collapses)
        {
            if (collapser != null && collapser.gameObject.CompareTag("Player")) {
                controller.takeDamage.Invoke(Damage);
            }
        }
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

