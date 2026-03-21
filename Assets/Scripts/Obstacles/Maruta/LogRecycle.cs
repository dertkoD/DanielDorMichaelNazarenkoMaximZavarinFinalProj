using UnityEngine;

public class LogRecycle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PooledLog>(out var pooledLog))
        {
            pooledLog.ReturnToPool();
        }
    }
}
