using UnityEngine;
using UnityEngine.AI;

public class EnemyDodgeController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform selfTransform;

    [Header("Dodge")]
    [SerializeField] private float dodgeDistance = 2.2f;
    [SerializeField] private float dodgeArriveDistance = 0.35f;
    [SerializeField] private float dodgeChancePerCheck = 0.18f;
    [SerializeField] private float dodgeCooldown = 2.5f;

    private float nextDodgeAllowedTime;
    private bool isDodging;
    private Vector3 currentDodgeTarget;

    public bool IsDodging => isDodging;

    public bool TryStartDodge(Transform target)
    {
        if (navMeshAgent == null || selfTransform == null || target == null)
            return false;

        if (Time.time < nextDodgeAllowedTime)
            return false;

        if (Random.value > dodgeChancePerCheck)
            return false;

        Vector3 toTarget = target.position - selfTransform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
            return false;

        Vector3 side = Vector3.Cross(Vector3.up, toTarget.normalized);
        if (Random.value < 0.5f)
            side = -side;

        Vector3 desired = selfTransform.position + side * dodgeDistance;

        if (!NavMesh.SamplePosition(desired, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
            return false;

        currentDodgeTarget = hit.position;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentDodgeTarget);

        isDodging = true;
        nextDodgeAllowedTime = Time.time + dodgeCooldown;
        return true;
    }

    public bool UpdateDodge()
    {
        if (!isDodging || navMeshAgent == null)
            return false;

        if (navMeshAgent.pathPending)
            return true;

        if (navMeshAgent.remainingDistance <= Mathf.Max(dodgeArriveDistance, navMeshAgent.stoppingDistance))
        {
            isDodging = false;
            return false;
        }

        return true;
    }

    public void CancelDodge()
    {
        isDodging = false;
    }
}
