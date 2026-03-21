using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider playerCollider;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float castPadding = 0.08f;
    [SerializeField] private float skin = 0.01f;
    [SerializeField] private float maxGroundAngle = 60f;
    [SerializeField] private bool drawDebug = false;

    public bool IsGrounded { get; private set; }
    public RaycastHit GroundHit { get; private set; }
    public Collider GroundCollider => GroundHit.collider;
    public Vector3 GroundNormal => IsGrounded ? GroundHit.normal : Vector3.up;
    public float GroundAngle => Vector3.Angle(GroundNormal, Vector3.up);

    private void FixedUpdate()
    {
        IsGrounded = ProbeGround();

        if (drawDebug && playerCollider != null)
        {
            Vector3 origin = GetCastOrigin();
            float distance = GetCastDistance();
            Debug.DrawLine(origin, origin + Vector3.down * distance, IsGrounded ? Color.green : Color.red, Time.fixedDeltaTime);
        }
    }

    private bool ProbeGround()
    {
        GroundHit = default;

        if (playerCollider == null)
            return false;

        Vector3 origin = GetCastOrigin();
        float radius = GetCastRadius();
        float distance = GetCastDistance();

        bool hit = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out RaycastHit groundHit,
            distance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hit)
            return false;

        float minDot = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        float dot = Vector3.Dot(groundHit.normal, Vector3.up);

        if (dot < minDot)
            return false;

        GroundHit = groundHit;
        return true;
    }

    private Vector3 GetCastOrigin()
    {
        Bounds bounds = playerCollider.bounds;
        float radius = GetCastRadius();

        return new Vector3(
            bounds.center.x,
            bounds.min.y + radius + skin,
            bounds.center.z
        );
    }

    private float GetCastRadius()
    {
        Bounds bounds = playerCollider.bounds;
        float radius = Mathf.Min(bounds.extents.x, bounds.extents.z);
        return Mathf.Max(0.01f, radius - skin);
    }

    private float GetCastDistance()
    {
        return castPadding + skin;
    }
}
