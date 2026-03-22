using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Combat Movement", story: "Move [Self] around [Target] while fighting", category: "Action", id: "f983ebbe038db9341a172951a91bf5af")]
public partial class UpdateCombatMovementAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        return UpdateMovement();
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    private Status UpdateMovement()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        if (Target == null || Target.Value == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead)
            return Status.Failure;

        actor.UpdateCombatMovement(Target.Value.transform);
        return Status.Success;
    }
}

