using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Advance Patrol Index", story: "If [Self] reached [PatrolPointPosition] then increment [PatrolIndex] and wrap around [PatrolPath] count.", category: "Action", id: "9b6e676b88ff0ab3d4245fa2fcb44ad6")]
public partial class AdvancePatrolIndexAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> PatrolPointPosition;
    [SerializeReference] public BlackboardVariable<int> PatrolIndex;
    [SerializeReference] public BlackboardVariable<GameObject> PatrolPath;
    [SerializeReference] public BlackboardVariable<float> PatrolArriveDistance;

    protected override Status OnStart()
    {
        if (Self == null || Self.Value == null)
        {
            Debug.LogError("Advance Patrol Index: Self is null");
            return Status.Failure;
        }

        if (PatrolPath == null || PatrolPath.Value == null)
        {
            Debug.LogError("Advance Patrol Index: PatrolPath is null");
            return Status.Failure;
        }

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null)
        {
            Debug.LogError("Advance Patrol Index: EnemyActor missing on Self");
            return Status.Failure;
        }

        EnemyPatrolPath path = PatrolPath.Value.GetComponent<EnemyPatrolPath>();
        if (path == null)
        {
            Debug.LogError("Advance Patrol Index: EnemyPatrolPath missing on PatrolPath");
            return Status.Failure;
        }

        if (path.Count == 0)
        {
            Debug.LogError("Advance Patrol Index: no patrol points");
            return Status.Failure;
        }

        if (actor.HasReached(PatrolPointPosition.Value, PatrolArriveDistance.Value))
        {
            int next = PatrolIndex.Value + 1;
            if (next >= path.Count)
                next = 0;

            PatrolIndex.Value = next;
        }

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

