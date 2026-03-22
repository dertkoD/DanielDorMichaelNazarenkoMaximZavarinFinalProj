using UnityEngine;

public class EnemyActor : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private int objectId;

    [Header("References")]
    [SerializeField] private Transform selfTransform;
    [SerializeField] private Health health;
    [SerializeField] private EnemyMotor motor;
    [SerializeField] private EnemyTargetSensor targetSensor;
    [SerializeField] private EnemyAimController aimController;
    [SerializeField] private EnemyWeaponController weaponController;
    [SerializeField] private EnemyPatrolPath patrolPath;
    [SerializeField] private EnemyCombatController combatController;
    [SerializeField] private EnemyDodgeController dodgeController;
    [SerializeField] private EnemyCombatMovementController combatMovementController;
    public EnemyCombatMovementController CombatMovementController => combatMovementController;

    public int ObjectId => objectId;
    public Transform SelfTransform => selfTransform != null ? selfTransform : transform;
    public Health Health => health;
    public EnemyMotor Motor => motor;
    public EnemyTargetSensor TargetSensor => targetSensor;
    public EnemyAimController AimController => aimController;
    public EnemyWeaponController WeaponController => weaponController;
    public EnemyPatrolPath PatrolPath => patrolPath;
    public EnemyCombatController CombatController => combatController;
    public EnemyDodgeController DodgeController => dodgeController;

    public bool IsDead => health != null && health.IsDead;

    public void ScanForTarget()
    {
        if (targetSensor == null) return;
        targetSensor.Scan();
    }
    
    public void UpdateCombatMovement(Transform target)
    {
        if (combatMovementController == null) return;
        combatMovementController.UpdateCombatMovement(target);
    }

    public void AimAt(Vector3 point)
    {
        if (aimController == null) return;
        aimController.AimAt(point);
    }
    
    private void Awake()
    {
        if (weaponController != null && health != null)
        {
            weaponController.SetObjectId(health.ObjectId);
        }
    }

    public void SetWeaponAimPoint(Vector3 point)
    {
        if (weaponController == null) return;
        weaponController.SetAimPoint(point);
    }

    public void StartFire()
    {
        if (weaponController == null) return;
        weaponController.StartFire();
    }

    public void StopFire()
    {
        if (weaponController == null) return;
        weaponController.StopFire();
    }

    public void Reload()
    {
        if (weaponController == null) return;
        weaponController.Reload();
    }

    public void MoveTo(Vector3 point)
    {
        if (motor == null) return;
        motor.MoveTo(point);
    }

    public void StopMoving()
    {
        if (motor == null) return;
        motor.Stop();
    }

    public bool HasReached(Vector3 point, float threshold)
    {
        if (motor == null) return true;
        return motor.HasReached(threshold);
    }
}
