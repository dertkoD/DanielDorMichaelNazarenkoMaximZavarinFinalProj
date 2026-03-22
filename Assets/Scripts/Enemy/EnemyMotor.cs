using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;

    private Vector3 lastDestination;
    private bool hasDestination;

    public NavMeshAgent Agent => navMeshAgent;

    private void Awake()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
        }
    }

    public void MoveTo(Vector3 worldPoint)
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
            return;

        if (!hasDestination || Vector3.Distance(lastDestination, worldPoint) > 0.1f)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(worldPoint);
            lastDestination = worldPoint;
            hasDestination = true;
        }
    }

    public void Stop()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
            return;

        navMeshAgent.isStopped = true;
        hasDestination = false;
    }

    public bool HasReached(float distanceThreshold)
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
            return true;

        if (navMeshAgent.pathPending)
            return false;

        float effectiveThreshold = Mathf.Max(distanceThreshold, navMeshAgent.stoppingDistance);
        return navMeshAgent.remainingDistance <= effectiveThreshold;
    }
}
