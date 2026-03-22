using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider playerCollider;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float castPadding = 0.08f;
    [SerializeField] private float skin = 0.01f;
    [SerializeField] private bool drawDebug = false;

    [Header("Ground Memory")]
    [SerializeField] private float groundedRememberTime = 0.08f;

    public bool IsGrounded { get; private set; }
    public RaycastHit GroundHit { get; private set; }
    public Collider GroundCollider => GroundHit.collider;
    public Vector3 GroundNormal => GroundHit.collider != null ? GroundHit.normal : Vector3.up;

    private float lastGroundedTime = -999f;
    private RaycastHit lastValidGroundHit;

    private void FixedUpdate()
    {
        bool foundGround = ProbeGround(out RaycastHit hit);

        if (foundGround)
        {
            IsGrounded = true;
            GroundHit = hit;
            lastValidGroundHit = hit;
            lastGroundedTime = Time.time;
        }
        else
        {
            bool withinMemory = Time.time - lastGroundedTime <= groundedRememberTime;

            IsGrounded = withinMemory;
            GroundHit = withinMemory ? lastValidGroundHit : default;
        }

        if (drawDebug && playerCollider != null)
        {
            Vector3 origin = GetCastOrigin();
            float distance = GetCastDistance();
            Debug.DrawLine(
                origin,
                origin + Vector3.down * distance,
                IsGrounded ? Color.green : Color.red,
                Time.fixedDeltaTime
            );
        }
    }

    private bool ProbeGround(out RaycastHit groundHit)
    {
        groundHit = default;

        if (playerCollider == null)
            return false;

        Vector3 origin = GetCastOrigin();
        float radius = GetCastRadius();
        float distance = GetCastDistance();

        return Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out groundHit,
            distance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
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