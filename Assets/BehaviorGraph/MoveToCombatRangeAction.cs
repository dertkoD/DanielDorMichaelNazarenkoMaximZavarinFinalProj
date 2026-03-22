using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Combat Range", story: "Move [Self] to combat range of [Target]", category: "Action", id: "f88265a9de77666399ea6dd5e2de69ed")]
public partial class MoveToCombatRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> PreferredCombatDistance;

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

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead || actor.CombatController == null)
            return Status.Failure;

        Vector3 targetPos = Target.Value.transform.position;
        float distance = Vector3.Distance(actor.SelfTransform.position, targetPos);

        if (distance <= PreferredCombatDistance.Value)
        {
            actor.StopMoving();
            return Status.Success;
        }

        actor.MoveTo(targetPos);
        return Status.Running;
    }
}

