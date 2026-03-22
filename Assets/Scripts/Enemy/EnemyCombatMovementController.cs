using UnityEngine;
using UnityEngine.AI;

public class EnemyCombatMovementController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform selfTransform;

    [Header("Distances")]
    [SerializeField] private float preferredDistance = 8f;
    [SerializeField] private float tooCloseDistance = 5.5f;
    [SerializeField] private float repathInterval = 0.2f;

    [Header("Orbit")]
    [SerializeField] private float orbitOffset = 2.0f;
    [SerializeField] private float orbitSwitchInterval = 1.5f;
    [SerializeField] private float orbitChance = 0.85f;

    private float nextRepathTime;
    private float nextOrbitDecisionTime;
    private int orbitSign = 1;

    public void UpdateCombatMovement(Transform target)
    {
        if (navMeshAgent == null || selfTransform == null || target == null)
            return;

        if (Time.time >= nextOrbitDecisionTime)
        {
            nextOrbitDecisionTime = Time.time + orbitSwitchInterval;

            if (Random.value < orbitChance)
                orbitSign = Random.value < 0.5f ? -1 : 1;
            else
                orbitSign = 0;
        }

        if (Time.time < nextRepathTime)
            return;

        nextRepathTime = Time.time + repathInterval;

        Vector3 selfPos = selfTransform.position;
        Vector3 targetPos = target.position;

        Vector3 toTarget = targetPos - selfPos;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        if (distance < 0.001f)
            return;

        Vector3 dir = toTarget / distance;
        Vector3 tangent = Vector3.Cross(Vector3.up, dir) * orbitSign;

        Vector3 desiredPosition;

        if (distance > preferredDistance + 0.75f)
        {
            desiredPosition = targetPos - dir * preferredDistance + tangent * orbitOffset;
        }
        else if (distance < tooCloseDistance)
        {
            desiredPosition = selfPos - dir * 2.5f + tangent * orbitOffset;
        }
        else
        {
            desiredPosition = selfPos + tangent * orbitOffset;
        }

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(hit.position);
        }
    }

    public void Stop()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled)
            return;

        navMeshAgent.isStopped = true;
    }
}
