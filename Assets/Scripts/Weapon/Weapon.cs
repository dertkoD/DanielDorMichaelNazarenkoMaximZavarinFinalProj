using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private BulletPool bulletPool;

    [Header("Fire Mode")]
    [SerializeField] private bool automatic = true;

    [Header("Fire Params")]
    [SerializeField] private float fireRate = 8f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifetime = 5f;

    [Header("Direction")]
    [SerializeField] private bool flattenDirectionToGround = true;

    private bool isFireHeld;
    private float nextFireTime;

    public Transform Muzzle => muzzle;
    public bool Automatic => automatic;
    public float FireRate => fireRate;

    private void Update()
    {
        if (!automatic) return;
        if (!isFireHeld) return;

        TryFireInternal();
    }

    public void StartFire()
    {
        isFireHeld = true;

        if (!automatic)
            TryFireInternal();
    }

    public void StopFire()
    {
        isFireHeld = false;
    }

    public void TryFireOnce()
    {
        TryFireInternal();
    }

    private void TryFireInternal()
    {
        if (muzzle == null) return;
        if (bulletPool == null) return;
        if (fireRate <= 0f) return;
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + (1f / fireRate);

        Vector3 direction = muzzle.forward;

        if (flattenDirectionToGround)
        {
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            if (direction.sqrMagnitude < 0.0001f)
                direction = transform.forward;
        }

        direction.Normalize();

        PooledBullet bullet = bulletPool.GetBullet(
            muzzle.position,
            Quaternion.LookRotation(direction)
        );

        bullet.Launch(direction, bulletSpeed, bulletLifetime);
    }
}
