using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LogPool logPool;
    [SerializeField] private Transform[] spawnSlots = new Transform[3];

    [Header("Timing")]
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float spawnInterval = 3f;
    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            List<int> chosenSlots = GetTwoRandomSlots();

            SpawnLogInSlot(chosenSlots[0]);
            SpawnLogInSlot(chosenSlots[1]);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnLogInSlot(int slotIndex)
    {
        GameObject log = logPool.GetLog();
        if (!log) return;

        Transform slot = spawnSlots[slotIndex];

        log.transform.SetPositionAndRotation(slot.position, slot.rotation);
        
        log.SetActive(true);
    }

    private List<int> GetTwoRandomSlots()
    {
        List<int> indices = new List<int> { 0, 1, 2 };

        for (int i = 0; i < indices.Count; i++)
        {
            int randomIndex = Random.Range(i, indices.Count);
            (indices[i], indices[randomIndex]) = (indices[randomIndex], indices[i]);
        }

        return new List<int> { indices[0], indices[1] };
    }
}
