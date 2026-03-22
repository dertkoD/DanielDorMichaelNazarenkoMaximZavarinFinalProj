using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Combat Aim", story: "Aim [Self] at [Target] and set weapon aim", category: "Action", id: "5f64ef0d403cb30ede5990101357b047")]
public partial class SetCombatAimAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    protected override Status OnStart()
    {
        return UpdateAim();
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    private Status UpdateAim()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        if (Target == null || Target.Value == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead || actor.CombatController == null)
            return Status.Failure;

        Transform targetTransform = Target.Value.transform;
        Vector3 aimPoint = actor.CombatController.GetAimPoint(targetTransform);

        TargetPosition.Value = aimPoint;
        actor.AimAt(aimPoint);
        actor.SetWeaponAimPoint(aimPoint);

        return Status.Success;
    }
}

