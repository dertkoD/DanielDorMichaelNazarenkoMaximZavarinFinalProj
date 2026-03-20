using System.Collections;
using UnityEngine;

public class ForwardShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private Transform firePoint;

    [Header("Start")]
    [SerializeField] private bool shootOnStart = true;
    [SerializeField] private bool loopShooting = true;

    [Header("Taimings")]
    [SerializeField] private float firstShotDelay = 0.5f;
    [SerializeField] private float fireRate = 1.0f;

    [Header("Burst")]
    [SerializeField] private bool useBurst = true;
    [SerializeField] private int bulletsPerBurst = 3;
    [SerializeField] private float burstBulletInterval = 0.05f;

    [Header("Bullet")]
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifeTime = 5f;

    private Coroutine shootCoroutine;

    private void Start()
    {
        if (shootOnStart)
            StartShooting();
    }

    public void StartShooting()
    {
        if (shootCoroutine == null)
            shootCoroutine = StartCoroutine(ShootingRoutine());
    }

    public void StopShooting()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator ShootingRoutine()
    {
        if (firstShotDelay > 0f)
            yield return new WaitForSeconds(firstShotDelay);

        do
        {
            if (useBurst)
                yield return StartCoroutine(FireBurst());
            else
                FireSingleBullet();

            if (fireRate > 0f)
                yield return new WaitForSeconds(fireRate);

        } while (loopShooting);

        shootCoroutine = null;
    }

    private IEnumerator FireBurst()
    {
        int count = Mathf.Max(1, bulletsPerBurst);

        for (int i = 0; i < count; i++)
        {
            FireSingleBullet();

            if (i < count - 1 && burstBulletInterval > 0f)
                yield return new WaitForSeconds(burstBulletInterval);
        }
    }

    private void FireSingleBullet()
    {
        Vector3 dir = Vector3.ProjectOnPlane(-firePoint.forward, Vector3.up);

        if (dir.sqrMagnitude < 0.0001f)
            dir = transform.forward;

        dir.Normalize();

        PooledBullet bullet = bulletPool.GetBullet(
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        bullet.Launch(dir, bulletSpeed, bulletLifeTime);
    }
}
