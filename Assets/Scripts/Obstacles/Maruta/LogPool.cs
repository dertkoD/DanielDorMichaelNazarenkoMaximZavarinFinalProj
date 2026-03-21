using System.Collections.Generic;
using UnityEngine;

public class LogPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject logPrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private PooledLog logScriptRef;

    private readonly Queue<GameObject> availableLogs = new Queue<GameObject>();
    private readonly List<GameObject> allLogs = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject log = Instantiate(logPrefab, transform);
            
            if (log.TryGetComponent<PooledLog>(out var pooledLog))
            {
                 pooledLog.SetPool(this);
            }
            
            log.SetActive(false);
            allLogs.Add(log);
            availableLogs.Enqueue(log); 
        }
    }

    public GameObject GetLog()
    {
        if (availableLogs.Count == 0) return null;

        GameObject log = availableLogs.Dequeue();
        return log;
    }

    public void ReturnLog(GameObject log)
    {
        log.SetActive(false);
        availableLogs.Enqueue(log);
    }
}
