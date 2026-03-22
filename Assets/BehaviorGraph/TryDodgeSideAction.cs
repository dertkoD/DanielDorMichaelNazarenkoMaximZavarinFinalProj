using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Try Dodge Side", story: "Try side dodge for [Self] around [Target]", category: "Action", id: "807eafb5a1e4b8631ef607f53ef716f3")]
public partial class TryDodgeSideAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        if (Target == null || Target.Value == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead || actor.DodgeController == null)
            return Status.Failure;

        bool started = actor.DodgeController.TryStartDodge(Target.Value.transform);
        return started ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead || actor.DodgeController == null)
            return Status.Failure;

        bool stillDodging = actor.DodgeController.UpdateDodge();
        return stillDodging ? Status.Running : Status.Success;
    }
}

