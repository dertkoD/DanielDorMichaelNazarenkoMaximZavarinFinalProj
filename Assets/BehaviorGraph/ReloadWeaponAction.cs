using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Reload Weapon", story: "Reload weapon for [Self]", category: "Action", id: "ad2a7d46c4d3f23913114b947670998d")]
public partial class ReloadWeaponAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead || actor.WeaponController == null)
            return Status.Failure;

        actor.Reload();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }
}

