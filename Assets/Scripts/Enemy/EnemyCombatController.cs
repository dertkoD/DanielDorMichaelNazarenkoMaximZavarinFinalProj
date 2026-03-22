using UnityEngine;

public class EnemyCombatController : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float preferredCombatDistance = 8f;
    [SerializeField] private float maxShootDistance = 11f;

    [Header("Aim Error")]
    [SerializeField] private float aimJitterRadius = 0.35f;
    [SerializeField] private float aimRefreshInterval = 0.15f;

    [Header("Aim Follow")]
    [SerializeField] private float targetFollowLerp = 8f;
    [SerializeField] private float lagDistanceFactor = 0.12f;

    [Header("Burst Feel")]
    [SerializeField] private float minFireBurstTime = 0.5f;
    [SerializeField] private float maxFireBurstTime = 1.2f;
    [SerializeField] private float minPauseBetweenBursts = 0.25f;
    [SerializeField] private float maxPauseBetweenBursts = 0.8f;

    private Vector3 smoothedTargetPosition;
    private Vector3 currentAimPoint;
    private float nextAimRefreshTime;

    private bool burstActive;
    private float burstStateEndTime;

    public float PreferredCombatDistance => preferredCombatDistance;
    public float MaxShootDistance => maxShootDistance;

    public Vector3 GetAimPoint(Transform target)
    {
        if (target == null)
            return transform.position;

        Vector3 rawTargetPos = target.position;

        if (smoothedTargetPosition == Vector3.zero)
            smoothedTargetPosition = rawTargetPos;

        smoothedTargetPosition = Vector3.Lerp(
            smoothedTargetPosition,
            rawTargetPos,
            targetFollowLerp * Time.deltaTime
        );

        Vector3 laggedTarget = Vector3.Lerp(
            rawTargetPos,
            smoothedTargetPosition,
            lagDistanceFactor
        );

        if (Time.time >= nextAimRefreshTime)
        {
            nextAimRefreshTime = Time.time + aimRefreshInterval;

            Vector2 jitter = Random.insideUnitCircle * aimJitterRadius;

            currentAimPoint = new Vector3(
                laggedTarget.x + jitter.x,
                laggedTarget.y,
                laggedTarget.z + jitter.y
            );
        }

        return currentAimPoint;
    }

    public bool WantsToFire()
    {
        if (Time.time >= burstStateEndTime)
        {
            burstActive = !burstActive;

            if (burstActive)
                burstStateEndTime = Time.time + Random.Range(minFireBurstTime, maxFireBurstTime);
            else
                burstStateEndTime = Time.time + Random.Range(minPauseBetweenBursts, maxPauseBetweenBursts);
        }

        return burstActive;
    }
}
