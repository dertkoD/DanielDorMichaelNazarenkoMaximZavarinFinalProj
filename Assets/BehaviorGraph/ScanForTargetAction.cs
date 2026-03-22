using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Scan For Target", story: "Scan for [target] using [Self] and update blackboard", category: "Action", id: "efedb8a5b7017bbe47b819827df87de7")]
public partial class ScanForTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownTargetPosition;
    [SerializeReference] public BlackboardVariable<bool> HasTarget;
    [SerializeReference] public BlackboardVariable<bool> HasLineOfSight;
    [SerializeReference] public BlackboardVariable<float> LostTargetMemoryTime;

    protected override Status OnStart()
    {
        return ScanNow();
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    private Status ScanNow()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead || actor.TargetSensor == null)
            return Status.Failure;

        if (LostTargetMemoryTime != null)
            actor.TargetSensor.SetLostTargetMemoryTime(LostTargetMemoryTime.Value);

        actor.ScanForTarget();

        if (actor.TargetSensor.HasTarget)
        {
            Target.Value = actor.TargetSensor.CurrentTarget.gameObject;
            TargetPosition.Value = actor.TargetSensor.CurrentTargetPoint;
            LastKnownTargetPosition.Value = actor.TargetSensor.CurrentTargetPoint;
            HasTarget.Value = true;
            HasLineOfSight.Value = actor.TargetSensor.HasLineOfSight;
        }
        else
        {
            Target.Value = null;
            HasTarget.Value = false;
            HasLineOfSight.Value = false;
        }

        return Status.Success;
    }
}

