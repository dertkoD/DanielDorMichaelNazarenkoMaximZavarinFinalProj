using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Weapon State", story: "Update reload state from [Self]", category: "Action", id: "1cce3faa7ce62767dda48977a8a8b8a0")]
public partial class UpdateWeaponStateAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> NeedsReload;
    [SerializeReference] public BlackboardVariable<bool> IsReloading;

    protected override Status OnStart()
    {
        if (Self == null || Self.Value == null)
            return Status.Failure;

        EnemyActor actor = Self.Value.GetComponent<EnemyActor>();
        if (actor == null || actor.IsDead || actor.WeaponController == null)
            return Status.Failure;

        NeedsReload.Value = actor.WeaponController.NeedsReload;
        IsReloading.Value = actor.WeaponController.IsReloading;

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }
}

