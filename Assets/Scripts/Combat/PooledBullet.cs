using UnityEngine;

public class PooledBullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;

    private float speed;
    private float timer;
    private Vector3 moveDirection;
    private BulletPool pool;
    private bool isActive;

    public void Init(BulletPool bulletPool)
    {
        pool = bulletPool;
    }

    public void Launch(Vector3 direction, float bulletSpeed, float bulletLifeTime)
    {
        moveDirection = direction.normalized;
        speed = bulletSpeed;
        lifeTime = bulletLifeTime;
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
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

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
