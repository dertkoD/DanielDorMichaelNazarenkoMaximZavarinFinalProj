using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Patrol Point", story: "Move [Self] to [PatrolPointPosition] using NavMeshAgent until within PatrolArriveDistance.", category: "Action", id: "3b5502efb28d17f325b9176948044d54")]
public partial class MoveToPatrolPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> PatrolPointPosition;
    [SerializeReference] public BlackboardVariable<float> PatrolArriveDistance;
    protected override Status OnStart()
    {
        if (Self == null || Self.Value == null)
        {
            Debug.LogError("Move To Patrol Point: Self is null");
            return Status.Failure;
        }

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null)
        {
            Debug.LogError("Move To Patrol Point: EnemyActor missing on Self");
            return Status.Failure;
        }

        if (actor.IsDead)
        {
            Debug.LogError("Move To Patrol Point: actor is dead");
            return Status.Failure;
        }

        actor.MoveTo(PatrolPointPosition.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead)
            return Status.Failure;

        actor.ScanForTarget();

        if (actor.TargetSensor != null && actor.TargetSensor.HasTarget)
        {
            actor.StopMoving();
            return Status.Failure;
        }

        actor.MoveTo(PatrolPointPosition.Value);

        if (actor.AimController != null)
            actor.AimController.LookMoveDirection();

        if (actor.HasReached(PatrolPointPosition.Value, PatrolArriveDistance.Value))
        {
            actor.StopMoving();
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

