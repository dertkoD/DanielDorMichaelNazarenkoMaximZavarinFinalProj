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

    private Vector3 currentAimPoint;
    private bool hasAimPoint;

    public Transform Muzzle => muzzle;
    public bool Automatic => automatic;
    public float FireRate => fireRate;

    private void Update()
    {
        if (!automatic) return;
        if (!isFireHeld) return;

        TryFireInternal();
    }

    public void SetAimPoint(Vector3 aimPoint)
    {
        currentAimPoint = aimPoint;
        hasAimPoint = true;
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
        if (!hasAimPoint) return;
        if (fireRate <= 0f) return;
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + (1f / fireRate);

        Vector3 direction;

        if (flattenDirectionToGround)
        {
            Vector3 flatAimPoint = new Vector3(currentAimPoint.x, muzzle.position.y, currentAimPoint.z);
            direction = flatAimPoint - muzzle.position;

            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector3.ProjectOnPlane(muzzle.forward, Vector3.up);
        }
        else
        {
            direction = currentAimPoint - muzzle.position;
        }

        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        PooledBullet bullet = bulletPool.GetBullet(
            muzzle.position,
            Quaternion.LookRotation(direction)
        );

        bullet.Launch(direction, bulletSpeed, bulletLifetime);
    }
}
