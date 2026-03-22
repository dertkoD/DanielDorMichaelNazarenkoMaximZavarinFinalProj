using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    [SerializeField] private int objectId;
    [SerializeField] private Weapon weapon;

    public Weapon Weapon => weapon;
    public bool HasWeapon => weapon != null;
    public bool IsReloading => weapon != null && weapon.IsReloading;
    public bool NeedsReload => weapon != null && weapon.NeedsReload;
    public bool CanShoot => weapon != null && weapon.CanShoot;

    private void Awake()
    {
        if (weapon != null)
            weapon.SetOwnerObjectId(objectId);
    }

    public void SetAimPoint(Vector3 worldPoint)
    {
        if (weapon == null) return;
        weapon.SetAimPoint(worldPoint);
    }

    public void StartFire()
    {
        if (weapon == null)
        {
            Debug.LogError($"{name}: EnemyWeaponController -> weapon is null");
            return;
        }

        Debug.Log($"{name}: EnemyWeaponController -> StartFire");
        weapon.StartFire();
    }

    public void StopFire()
    {
        if (weapon == null) return;
        weapon.StopFire();
    }

    public void Reload()
    {
        if (weapon == null) return;
        weapon.Reload();
    }
}
