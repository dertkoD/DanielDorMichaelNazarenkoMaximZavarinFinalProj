using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbodyComponent;
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] private Transform movementReference;

    [Header("Speed")]
    [SerializeField] private float groundSpeed = 6f;
    [SerializeField] private float airSpeed = 4.5f;

    [Header("Control")]
    [SerializeField] private float acceleration = 30f;

    [Header("Inertia / Ice")]
    [SerializeField] private float groundSlideDecay = 2.5f;
    [SerializeField] private float airSlideDecay = 0.5f;

    [Header("Forced Slope Slide")]
    [SerializeField] private float slideStartAngle = 35f;
    [SerializeField] private float slopeSlideAcceleration = 10f;
    [SerializeField] private float maxSlopeSlideSpeed = 7f;

    [Header("Dirt")]
    [SerializeField] private string dirtLayerName = "Dirt";
    [SerializeField] private float dirtSpeedMultiplier = 0.45f;
    [SerializeField] private float dirtAccelerationMultiplier = 0.65f;
    [SerializeField] private float dirtSlideDecayMultiplier = 1.4f;

    [Header("Grounding")]
    [SerializeField] private float groundedDownForce = 5f;

    private Vector2 moveInput;

    public void SetMoveInput(Vector2 input)
    {
        moveInput = Vector2.ClampMagnitude(input, 1f);
    }

    private void FixedUpdate()
    {
        if (rigidbodyComponent == null)
            return;

        bool isGrounded = groundChecker != null && groundChecker.IsGrounded;
        Vector3 planeNormal = isGrounded ? groundChecker.GroundHit.normal : Vector3.up;

        Vector3 velocity = rigidbodyComponent.linearVelocity;
        Vector3 planarVelocity = Vector3.ProjectOnPlane(velocity, planeNormal);
        Vector3 verticalVelocity = velocity - planarVelocity;

        Vector3 moveDirection = GetMoveDirection(moveInput, planeNormal);

        float currentGroundSpeed = groundSpeed;
        float currentAcceleration = acceleration;
        float currentGroundSlideDecay = groundSlideDecay;

        if (IsOnDirtSurface())
        {
            currentGroundSpeed *= dirtSpeedMultiplier;
            currentAcceleration *= dirtAccelerationMultiplier;
            currentGroundSlideDecay *= dirtSlideDecayMultiplier;
        }

        float targetSpeed = isGrounded ? currentGroundSpeed : airSpeed;

        Vector3 newPlanarVelocity = planarVelocity;

        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 targetPlanarVelocity = moveDirection * targetSpeed;

            float usedAcceleration = isGrounded ? currentAcceleration : acceleration;

            newPlanarVelocity = Vector3.MoveTowards(
                planarVelocity,
                targetPlanarVelocity,
                usedAcceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            float decay = isGrounded ? currentGroundSlideDecay : airSlideDecay;

            newPlanarVelocity = Vector3.MoveTowards(
                planarVelocity,
                Vector3.zero,
                decay * Time.fixedDeltaTime
            );
        }

        if (isGrounded && ShouldSlide(out Vector3 slopeSlideDirection))
        {
            Vector3 slopeSlideVelocity = Vector3.ProjectOnPlane(newPlanarVelocity, planeNormal);

            Vector3 slopeOnlyVelocity = Vector3.Project(
                slopeSlideVelocity,
                slopeSlideDirection
            );

            Vector3 acceleratedSlopeVelocity = Vector3.MoveTowards(
                slopeOnlyVelocity,
                slopeSlideDirection * maxSlopeSlideSpeed,
                slopeSlideAcceleration * Time.fixedDeltaTime
            );

            Vector3 sidewaysVelocity = slopeSlideVelocity - slopeOnlyVelocity;
            newPlanarVelocity = sidewaysVelocity + acceleratedSlopeVelocity;
        }

        Vector3 finalVelocity = newPlanarVelocity + verticalVelocity;

        if (isGrounded && finalVelocity.y <= 0f)
            finalVelocity += Vector3.down * groundedDownForce * Time.fixedDeltaTime;

        rigidbodyComponent.linearVelocity = finalVelocity;
    }

    private bool IsOnDirtSurface()
    {
        if (groundChecker == null || !groundChecker.IsGrounded)
            return false;

        Collider groundCollider = groundChecker.GroundCollider;
        if (groundCollider == null)
            return false;

        int dirtLayer = LayerMask.NameToLayer(dirtLayerName);
        if (dirtLayer < 0)
            return false;

        return groundCollider.gameObject.layer == dirtLayer;
    }

    private Vector3 GetMoveDirection(Vector2 input, Vector3 planeNormal)
    {
        if (input.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        Vector3 rawDirection = new Vector3(input.x, 0f, input.y);

        rawDirection = Vector3.ProjectOnPlane(rawDirection, planeNormal);

        if (rawDirection.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        return rawDirection.normalized;
    }

    private bool ShouldSlide(out Vector3 slideDirection)
    {
        slideDirection = Vector3.zero;

        if (groundChecker == null || !groundChecker.IsGrounded)
            return false;

        Vector3 groundNormal = groundChecker.GroundHit.normal;
        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);

        if (slopeAngle < slideStartAngle)
            return false;

        slideDirection = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;
        return slideDirection.sqrMagnitude > 0.0001f;
    }

    public Vector2 MoveInput => moveInput;

    public Vector3 CurrentVelocity =>
        rigidbodyComponent != null ? rigidbodyComponent.linearVelocity : Vector3.zero;

    public Vector3 HorizontalVelocity
    {
        get
        {
            if (rigidbodyComponent == null)
                return Vector3.zero;

            Vector3 velocity = rigidbodyComponent.linearVelocity;
            velocity.y = 0f;
            return velocity;
        }
    }
}