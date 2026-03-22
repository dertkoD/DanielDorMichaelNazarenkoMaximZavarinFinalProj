using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get Patrol Point Position", story: "Read [PatrolPath] and [PatrolIndex] and write the current patrol point position into PatrolPointPosition.", category: "Action", id: "0f2def998e8700d7e1a380665ea8103f")]
public partial class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> PatrolPath;
    [SerializeReference] public BlackboardVariable<int> PatrolIndex;
    [SerializeReference] public BlackboardVariable<Vector3> PatrolPointPosition;
    [SerializeReference] public BlackboardVariable<float> PatrolArriveDistance;

    protected override Status OnStart()
    {
        if (Self == null || Self.Value == null)
        {
            Debug.LogError("Get Patrol Point Position: Self is null");
            return Status.Failure;
        }

        if (PatrolPath == null || PatrolPath.Value == null)
        {
            Debug.LogError("Get Patrol Point Position: PatrolPath is null");
            return Status.Failure;
        }

        EnemyPatrolPath path = PatrolPath.Value.GetComponent<EnemyPatrolPath>();
        if (path == null)
        {
            Debug.LogError("Get Patrol Point Position: EnemyPatrolPath component missing on PatrolPath object");
            return Status.Failure;
        }

        if (path.Count == 0)
        {
            Debug.LogError("Get Patrol Point Position: PatrolPath has 0 patrol points");
            return Status.Failure;
        }

        int index = PatrolIndex.Value;
        if (index < 0)
            index = 0;

        if (index >= path.Count)
            index = 0;

        Transform point = path.GetPoint(index);
        if (point == null)
        {
            Debug.LogError($"Get Patrol Point Position: patrol point at index {index} is null");
            return Status.Failure;
        }

        PatrolIndex.Value = index;
        PatrolPointPosition.Value = point.position;

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