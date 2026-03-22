using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot Target", story: "Shoot [Target] with [Self] when possible", category: "Action", id: "a163b015e97768b29edbeeaec3a37e80")]
public partial class ShootTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> HasLineOfSight;
    [SerializeReference] public BlackboardVariable<float> MaxShootDistance;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownTargetPosition;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        if (Target == null || Target.Value == null)
            return Status.Failure;

        if (HasLineOfSight == null || MaxShootDistance == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead || actor.CombatController == null || actor.WeaponController == null)
            return Status.Failure;

        Transform targetTransform = Target.Value.transform;
        if (targetTransform == null)
            return Status.Failure;

        Vector3 followPoint;

        if (HasLineOfSight.Value)
        {
            followPoint = actor.CombatController.GetAimPoint(targetTransform);

            if (TargetPosition != null)
                TargetPosition.Value = followPoint;

            if (LastKnownTargetPosition != null)
                LastKnownTargetPosition.Value = followPoint;
        }
        else
        {
            followPoint = LastKnownTargetPosition != null
                ? LastKnownTargetPosition.Value
                : targetTransform.position;
        }

        actor.AimAt(followPoint);
        actor.SetWeaponAimPoint(followPoint);
        actor.UpdateCombatMovement(targetTransform);

        float distance = Vector3.Distance(actor.SelfTransform.position, targetTransform.position);
        if (distance > MaxShootDistance.Value)
        {
            actor.StopFire();
            return Status.Running;
        }

        if (!HasLineOfSight.Value)
        {
            actor.StopFire();
            return Status.Running;
        }

        if (actor.WeaponController.NeedsReload)
        {
            actor.StopFire();
            actor.Reload();
            return Status.Running;
        }

        if (actor.WeaponController.IsReloading)
        {
            actor.StopFire();
            return Status.Running;
        }

        if (actor.CombatController.WantsToFire())
            actor.StartFire();
        else
            actor.StopFire();

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (Self != null && Self.Value != null)
        {
            EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
            if (actor != null)
                actor.StopFire();
        }
    }
}

