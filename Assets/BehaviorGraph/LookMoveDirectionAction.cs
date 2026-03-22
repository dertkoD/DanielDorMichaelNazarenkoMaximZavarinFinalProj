using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Look Move Direction", story: "Rotate [Self] toward movement direction", category: "Action", id: "330c78112b8e8ce683c59926bd5d1cdf")]
public partial class LookMoveDirectionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        return UpdateLook();
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    private Status UpdateLook()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead || actor.AimController == null)
            return Status.Failure;

        actor.AimController.LookMoveDirection();
        return Status.Success;
    }
}

