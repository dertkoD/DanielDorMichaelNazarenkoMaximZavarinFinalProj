using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private ParticleSystem shootPartical;

    [Header("Fire Mode")]
    [SerializeField] private bool automatic = true;

    [Header("Fire Params")]
    [SerializeField] private float fireRate = 8f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private int bulletDamage = 10;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadDuration = 1.4f;
    [SerializeField] private bool autoReloadOnEmpty = true;
    [SerializeField] private bool canFireWhileReloading = false;

    [Header("Direction")]
    [SerializeField] private bool flattenDirectionToGround = true;

    private bool isFireHeld;
    private float nextFireTime;

    private Vector3 currentAimPoint;
    private bool hasAimPoint;
    private int ownerObjectId = -1;

    private int currentAmmoInMagazine;
    private bool isReloading;
    private float reloadFinishTime;

    public Transform Muzzle => muzzle;
    public bool Automatic => automatic;
    public float FireRate => fireRate;

    public int MagazineSize => magazineSize;
    public int CurrentAmmoInMagazine => currentAmmoInMagazine;
    public bool IsReloading => isReloading;
    public bool IsMagazineEmpty => currentAmmoInMagazine <= 0;
    public bool CanShoot => !isReloading && currentAmmoInMagazine > 0;
    public bool NeedsReload => currentAmmoInMagazine <= 0 && !isReloading;

    private void Awake()
    {
        currentAmmoInMagazine = Mathf.Max(0, magazineSize);
    }

    private void Update()
    {
        TickReload();

        if (!automatic) return;
        if (!isFireHeld) return;

        TryFireInternal();
    }

    public void SetOwnerObjectId(int value)
    {
        ownerObjectId = value;
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

    public void Reload()
    {
        if (isReloading) return;
        if (currentAmmoInMagazine >= magazineSize) return;
        if (magazineSize <= 0) return;

        isReloading = true;
        reloadFinishTime = Time.time + reloadDuration;
    }

    private void TickReload()
    {
        if (!isReloading) return;
        if (Time.time < reloadFinishTime) return;

        isReloading = false;
        currentAmmoInMagazine = magazineSize;
    }

    private void TryFireInternal()
    {
        if (muzzle == null)
        {
            Debug.LogError($"{name}: Weapon fire blocked -> muzzle is null");
            return;
        }

        if (bulletPool == null)
        {
            Debug.LogError($"{name}: Weapon fire blocked -> bulletPool is null");
            return;
        }

        if (!hasAimPoint)
        {
            Debug.LogError($"{name}: Weapon fire blocked -> hasAimPoint is false");
            return;
        }

        if (fireRate <= 0f)
        {
            Debug.LogError($"{name}: Weapon fire blocked -> fireRate <= 0");
            return;
        }

        if (isReloading && !canFireWhileReloading)
        {
            Debug.Log($"{name}: Weapon fire blocked -> reloading");
            return;
        }

        if (currentAmmoInMagazine <= 0)
        {
            Debug.Log($"{name}: Weapon fire blocked -> empty magazine");

            if (autoReloadOnEmpty)
                Reload();

            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

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
        {
            Debug.LogError($"{name}: Weapon fire blocked -> direction too small");
            return;
        }

        direction.Normalize();

        PooledBullet bullet = bulletPool.GetBullet(
            muzzle.position,
            Quaternion.LookRotation(direction)
        );

        bullet.Launch(
            direction,
            bulletSpeed,
            bulletLifetime,
            bulletDamage,
            ownerObjectId
        );
        
        shootPartical.Play();
        AudioManager.Instance.PlayShootSounds();        

        currentAmmoInMagazine--;

        Debug.Log($"{name}: FIRE! ammo left = {currentAmmoInMagazine}");
    }
}
