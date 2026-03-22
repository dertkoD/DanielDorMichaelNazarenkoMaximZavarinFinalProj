using UnityEngine;

public class EnemyTargetSensor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform eyesPoint;

    [Header("View")]
    [SerializeField] private float viewRadius = 12f;
    [SerializeField, Range(0f, 360f)] private float viewAngle = 120f;

    [Header("Memory")]
    [SerializeField] private float lostTargetMemoryTime = 2f;

    [Header("Masks")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Debug")]
    [SerializeField] private bool drawDebug = true;

    public Transform CurrentTarget { get; private set; }
    public Vector3 CurrentTargetPoint { get; private set; }
    public bool HasTarget => CurrentTarget != null;
    public bool HasLineOfSight { get; private set; }

    private float lastSeenTime = -999f;

    public void SetLostTargetMemoryTime(float value)
    {
        lostTargetMemoryTime = Mathf.Max(0f, value);
    }

    public void Scan()
    {
        Transform visibleTarget = null;
        Vector3 visiblePoint = Vector3.zero;
        float bestDistance = float.MaxValue;

        HasLineOfSight = false;

        if (eyesPoint != null)
        {
            Collider[] hits = Physics.OverlapSphere(
                eyesPoint.position,
                viewRadius,
                targetMask,
                QueryTriggerInteraction.Ignore
            );

            for (int i = 0; i < hits.Length; i++)
            {
                Collider candidate = hits[i];
                if (candidate == null) continue;

                Vector3 targetPoint = candidate.bounds.center;
                Vector3 toTarget = targetPoint - eyesPoint.position;
                float distance = toTarget.magnitude;

                if (distance < 0.001f)
                    continue;

                Vector3 dir = toTarget / distance;
                float angle = Vector3.Angle(eyesPoint.forward, dir);

                if (angle > viewAngle * 0.5f)
                    continue;

                bool blocked = Physics.Raycast(
                    eyesPoint.position,
                    dir,
                    distance,
                    obstacleMask,
                    QueryTriggerInteraction.Ignore
                );

                if (blocked)
                    continue;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    visibleTarget = candidate.transform;
                    visiblePoint = targetPoint;
                }
            }
        }

        if (visibleTarget != null)
        {
            CurrentTarget = visibleTarget;
            CurrentTargetPoint = visiblePoint;
            HasLineOfSight = true;
            lastSeenTime = Time.time;
            return;
        }

        if (CurrentTarget != null)
        {
            HasLineOfSight = false;

            if (Time.time - lastSeenTime <= lostTargetMemoryTime)
            {
                CurrentTargetPoint = CurrentTarget.position;
                return;
            }
        }

        CurrentTarget = null;
        CurrentTargetPoint = Vector3.zero;
        HasLineOfSight = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebug || eyesPoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyesPoint.position, viewRadius);

        Vector3 left = GetDirectionFromAngle(-viewAngle * 0.5f);
        Vector3 right = GetDirectionFromAngle(viewAngle * 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(eyesPoint.position, eyesPoint.position + left * viewRadius);
        Gizmos.DrawLine(eyesPoint.position, eyesPoint.position + right * viewRadius);

        Gizmos.color = HasTarget ? Color.red : Color.green;
        Gizmos.DrawLine(eyesPoint.position, eyesPoint.position + eyesPoint.forward * viewRadius);

        if (HasTarget)
        {
            Gizmos.color = HasLineOfSight ? Color.red : new Color(1f, 0.5f, 0f);
            Gizmos.DrawLine(eyesPoint.position, CurrentTargetPoint);
            Gizmos.DrawSphere(CurrentTargetPoint, 0.12f);
        }
    }

    private Vector3 GetDirectionFromAngle(float angleOffset)
    {
        float angle = eyesPoint.eulerAngles.y + angleOffset;
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }
}
