using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private PooledBullet bulletPrefab;
    [SerializeField] private int initialSize = 20;
    [SerializeField] private Transform poolContainer;

    private readonly Queue<PooledBullet> pool = new Queue<PooledBullet>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateBullet();
        }
    }

    private PooledBullet CreateBullet()
    {
        PooledBullet bullet = Instantiate(
            bulletPrefab,
            poolContainer != null ? poolContainer : transform
        );

        bullet.Init(this);
        bullet.gameObject.SetActive(false);
        pool.Enqueue(bullet);

        return bullet;
    }

    public PooledBullet GetBullet(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0)
        {
            CreateBullet();
        }

        PooledBullet bullet = pool.Dequeue();
        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.gameObject.SetActive(true);

        return bullet;
    }

    public void ReturnBullet(PooledBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        pool.Enqueue(bullet);
    }
}
