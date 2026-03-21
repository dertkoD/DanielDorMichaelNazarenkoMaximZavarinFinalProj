using UnityEngine;

public class PooledBullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private DamageEventChannelSO damageEventChannel;

    private float speed;
    private float timer;
    private int damage;
    private int sourceObjectId;

    private Vector3 moveDirection;
    private BulletPool pool;
    private bool isActive;

    public void Init(BulletPool bulletPool)
    {
        pool = bulletPool;
    }

    public void Launch(
        Vector3 direction,
        float bulletSpeed,
        float bulletLifeTime,
        int bulletDamage,
        int ownerObjectId)
    {
        moveDirection = direction.normalized;
        speed = bulletSpeed;
        lifeTime = bulletLifeTime;
        damage = bulletDamage;
        sourceObjectId = ownerObjectId;

        timer = 0f;
        isActive = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isActive) return;

        transform.position += moveDirection * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (other.isTrigger) return;

        if (HealthHitboxRegistry.TryGet(other, out var hitbox))
        {
            int targetObjectId = hitbox.ObjectId;

            if (targetObjectId >= 0 && targetObjectId != sourceObjectId)
            {
                int finalDamage = Mathf.Max(
                    0,
                    Mathf.RoundToInt(damage * hitbox.DamageMultiplier)
                );

                if (finalDamage > 0 && damageEventChannel != null)
                {
                    damageEventChannel.Raise(new DamageInfo
                    {
                        sourceObjectId = sourceObjectId,
                        targetObjectId = targetObjectId,
                        damage = finalDamage,
                        hitPoint = transform.position,
                        hitNormal = -moveDirection
                    });
                }
            }
        }

        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (!isActive) return;
        if (pool == null) return;

        isActive = false;
        pool.ReturnBullet(this);
    }
}
