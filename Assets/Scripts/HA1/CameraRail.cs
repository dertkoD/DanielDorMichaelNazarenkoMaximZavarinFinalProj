using UnityEngine;

public class CameraRail : MonoBehaviour
{
    [Header("Rail points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Target")]
    public Transform target;

    [Header("Movement")]
    public float smoothTime = 0.12f;
    public bool clampToRail = true;
    public bool followOnlyForward = true;

    [Range(0f, 1f)]
    public float t = 0f;

    private float _tVelocity;
    private bool isGameActive = true;

    private float _cameraToTargetOffsetT;
    private Vector3 _railOffset;

    private void Start()
    {
        if (!startPoint || !endPoint || !target) return;

        float cameraT = GetProjectedT(transform.position);
        float targetT = GetProjectedT(target.position);

        _cameraToTargetOffsetT = cameraT - targetT;
        t = cameraT;

        Vector3 cameraRailPoint = Vector3.Lerp(startPoint.position, endPoint.position, cameraT);
        _railOffset = transform.position - cameraRailPoint;
    }

    private void LateUpdate()
    {
        if (!startPoint || !endPoint || !target) return;
        if (!isGameActive) return;

        float targetT = GetProjectedT(target.position);
        float desiredT = targetT + _cameraToTargetOffsetT;

        if (clampToRail)
            desiredT = Mathf.Clamp01(desiredT);

        if (followOnlyForward)
            desiredT = Mathf.Max(t, desiredT);

        t = Mathf.SmoothDamp(t, desiredT, ref _tVelocity, smoothTime);

        Vector3 railPos = Vector3.Lerp(startPoint.position, endPoint.position, t);
        transform.position = railPos + _railOffset;
    }

    private float GetProjectedT(Vector3 worldPos)
    {
        Vector3 rail = endPoint.position - startPoint.position;
        float railLengthSqr = rail.sqrMagnitude;

        if (railLengthSqr < 0.0001f)
            return 0f;

        Vector3 fromStart = worldPos - startPoint.position;
        float projectedT = Vector3.Dot(fromStart, rail) / railLengthSqr;

        if (clampToRail)
            projectedT = Mathf.Clamp01(projectedT);

        return projectedT;
    }

    public void SetGameActive(bool active)
    {
        isGameActive = active;
    }
}