using UnityEngine;

public class PooledLog : MonoBehaviour
{
    [SerializeField] private Rigidbody logRb;
    private LogPool pool;

    public void SetPool(LogPool assignedPool)
    {
        pool = assignedPool;
    }

    public void ReturnToPool()
    {
        
        if (pool)
        {
            pool.ReturnLog(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
        if (logRb)
        {
            logRb.linearVelocity = Vector3.zero;
            logRb.angularVelocity = Vector3.zero;
        }
    }
}
