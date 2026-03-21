using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private Collider playerCollider;

    [Header("Ground")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float extraDistance = 0.08f;
    [SerializeField] private float skin = 0.01f;
    [SerializeField] private float maxSlopeAngle = 60f;
    [SerializeField] private bool drawDebug = false;
    
    public bool IsGrounded { get; private set; }
    public RaycastHit GroundHit => _hit;

    public Collider GroundCollider => _hit.collider;
    
    private RaycastHit _hit;

    private void FixedUpdate()
    {
        IsGrounded = CheckGroundedCast();

        if (drawDebug)
        {
            Vector3 origin = GetCastOrigin();
            Vector3 end = origin + Vector3.down * GetCastDistance();
            Debug.DrawLine(origin, end, IsGrounded ? Color.green : Color.red, Time.fixedDeltaTime);
        }
    }

    private bool CheckGroundedCast()
    {
        if (playerCollider == null)
            return false;

        Vector3 origin = GetCastOrigin();
        float radius = GetCastRadius();
        float distance = GetCastDistance();

        bool hasHit = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out _hit,
            distance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hasHit)
            return false;

        float minDot = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        float dot = Vector3.Dot(_hit.normal, Vector3.up);

        return dot >= minDot;
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

    private float GetCastDistance()
    {
        float radius = GetCastRadius();
        float cos = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        float slopeCompensation = radius * ((1f / cos) - 1f);

        return extraDistance + skin + slopeCompensation;
    }

    private float GetCastRadius()
    {
        Bounds bounds = playerCollider.bounds;

        float radius = Mathf.Min(bounds.extents.x, bounds.extents.z);
        radius = Mathf.Max(0.01f, radius - skin);

        return radius;
    }
}
