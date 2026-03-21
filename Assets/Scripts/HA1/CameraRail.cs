using UnityEngine;

public class CameraRail : MonoBehaviour
{
    [Header("Rail points")]
    [SerializeField] private Transform[] railPoints;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private bool clampToRail = true;
    [SerializeField] private bool followOnlyForward = true;

    [Range(0f, 1f)]
    [SerializeField] private float t = 0f;

    private float _tVelocity;
    private bool isGameActive = true;

    private float _cameraToTargetOffsetT;
    private Vector3 _railOffset;

    private void Start()
    {
        if (!HasValidSetup())
            return;

        float cameraT = GetProjectedT(transform.position);
        float targetT = GetProjectedT(target.position);

        _cameraToTargetOffsetT = cameraT - targetT;
        t = cameraT;

        Vector3 cameraRailPoint = EvaluateRailPosition(cameraT);
        _railOffset = transform.position - cameraRailPoint;
    }

    private void LateUpdate()
    {
        if (!HasValidSetup())
            return;

        if (!isGameActive)
            return;

        float targetT = GetProjectedT(target.position);
        float desiredT = targetT + _cameraToTargetOffsetT;

        if (clampToRail)
            desiredT = Mathf.Clamp01(desiredT);

        if (followOnlyForward)
            desiredT = Mathf.Max(t, desiredT);

        t = Mathf.SmoothDamp(t, desiredT, ref _tVelocity, smoothTime);

        if (clampToRail)
            t = Mathf.Clamp01(t);

        Vector3 railPos = EvaluateRailPosition(t);
        transform.position = railPos + _railOffset;
    }

    private bool HasValidSetup()
    {
        return target != null && railPoints != null && railPoints.Length >= 2;
    }

    private float GetProjectedT(Vector3 worldPos)
    {
        float totalLength = GetTotalRailLength();
        if (totalLength <= 0.0001f)
            return 0f;

        float bestDistanceSqr = float.MaxValue;
        float bestGlobalT = 0f;

        float accumulatedLength = 0f;

        for (int i = 0; i < railPoints.Length - 1; i++)
        {
            Vector3 a = railPoints[i].position;
            Vector3 b = railPoints[i + 1].position;
            Vector3 segment = b - a;

            float segmentLength = segment.magnitude;
            if (segmentLength <= 0.0001f)
                continue;

            float segmentLengthSqr = segment.sqrMagnitude;
            float localT = Vector3.Dot(worldPos - a, segment) / segmentLengthSqr;

            if (clampToRail)
                localT = Mathf.Clamp01(localT);

            Vector3 projectedPoint = Vector3.Lerp(a, b, localT);
            float distanceSqr = (worldPos - projectedPoint).sqrMagnitude;

            if (distanceSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distanceSqr;

                float distanceAlongRail = accumulatedLength + localT * segmentLength;
                bestGlobalT = distanceAlongRail / totalLength;
            }

            accumulatedLength += segmentLength;
        }

        if (clampToRail)
            bestGlobalT = Mathf.Clamp01(bestGlobalT);

        return bestGlobalT;
    }

    private Vector3 EvaluateRailPosition(float globalT)
    {
        if (railPoints == null || railPoints.Length == 0)
            return transform.position;

        if (railPoints.Length == 1)
            return railPoints[0].position;

        float totalLength = GetTotalRailLength();
        if (totalLength <= 0.0001f)
            return railPoints[0].position;

        globalT = Mathf.Clamp01(globalT);
        float targetDistance = globalT * totalLength;

        float accumulatedLength = 0f;

        for (int i = 0; i < railPoints.Length - 1; i++)
        {
            Vector3 a = railPoints[i].position;
            Vector3 b = railPoints[i + 1].position;

            float segmentLength = Vector3.Distance(a, b);
            if (segmentLength <= 0.0001f)
                continue;

            if (targetDistance <= accumulatedLength + segmentLength)
            {
                float localDistance = targetDistance - accumulatedLength;
                float localT = localDistance / segmentLength;
                return Vector3.Lerp(a, b, localT);
            }

            accumulatedLength += segmentLength;
        }

        return railPoints[railPoints.Length - 1].position;
    }

    private float GetTotalRailLength()
    {
        if (railPoints == null || railPoints.Length < 2)
            return 0f;

        float total = 0f;

        for (int i = 0; i < railPoints.Length - 1; i++)
        {
            total += Vector3.Distance(railPoints[i].position, railPoints[i + 1].position);
        }

        return total;
    }

    public void SetGameActive(bool active)
    {
        isGameActive = active;
    }
}