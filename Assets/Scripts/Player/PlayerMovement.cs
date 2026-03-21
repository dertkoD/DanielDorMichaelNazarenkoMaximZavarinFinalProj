using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbodyComponent;
    [SerializeField] private Transform movementReference;
    [SerializeField] private GroundChecker groundChecker;

    [Header("Speed")]
    [SerializeField] private float maxGroundSpeed = 6f;
    [SerializeField] private float maxAirSpeed = 4.5f;

    [Header("Ground")]
    [SerializeField] private float groundAcceleration = 18f;
    [SerializeField] private float groundDeceleration = 10f;
    [SerializeField] private float groundTurnAcceleration = 8f;

    [Header("Air")]
    [SerializeField] private float airAcceleration = 6f;
    [SerializeField] private float airDeceleration = 2f;
    [SerializeField] private float airTurnAcceleration = 3f;
    
    [Header("Slope Slide")]
    [SerializeField] private float slideStartAngle = 35f;
    [SerializeField] private float slideAcceleration = 8f;
    [SerializeField] private float maxSlideSpeed = 5f;

    private Vector2 moveInput;

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        if (rigidbodyComponent == null)
            return;

        bool isGrounded = groundChecker != null && groundChecker.IsGrounded;

        Vector3 desiredDirection = GetMoveDirection(moveInput);
        Vector3 velocity = rigidbodyComponent.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        float maxSpeed = isGrounded ? maxGroundSpeed : maxAirSpeed;
        Vector3 targetVelocity = desiredDirection * maxSpeed;

        float acceleration = isGrounded ? groundAcceleration : airAcceleration;
        float deceleration = isGrounded ? groundDeceleration : airDeceleration;
        float turnAcceleration = isGrounded ? groundTurnAcceleration : airTurnAcceleration;

        if (desiredDirection.sqrMagnitude > 0.0001f)
        {
            float usedAcceleration = acceleration;

            if (horizontalVelocity.sqrMagnitude > 0.0001f)
            {
                float dot = Vector3.Dot(horizontalVelocity.normalized, desiredDirection);

                if (dot < 0f)
                    usedAcceleration = turnAcceleration;
            }

            horizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity,
                targetVelocity,
                usedAcceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            if (isGrounded && groundChecker != null && ShouldSlide(out Vector3 slideDirection))
            {
                Vector3 slideTargetVelocity = slideDirection * maxSlideSpeed;

                horizontalVelocity = Vector3.MoveTowards(
                    horizontalVelocity,
                    slideTargetVelocity,
                    slideAcceleration * Time.fixedDeltaTime
                );
            }
            else
            {
                horizontalVelocity = Vector3.MoveTowards(
                    horizontalVelocity,
                    Vector3.zero,
                    deceleration * Time.fixedDeltaTime
                );
            }
        }

        rigidbodyComponent.linearVelocity = new Vector3(
                horizontalVelocity.x,
                rigidbodyComponent.linearVelocity.y,
                horizontalVelocity.z
                );
    }
    
    private bool ShouldSlide(out Vector3 slideDirection)
    {
        slideDirection = Vector3.zero;

        RaycastHit hit = groundChecker.GroundHit;
        Vector3 groundNormal = hit.normal;

        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);
        if (slopeAngle < slideStartAngle)
            return false;

        slideDirection = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;

        return slideDirection.sqrMagnitude > 0.0001f;
    }

    private Vector3 GetMoveDirection(Vector2 input) 
    { 
        if (input.sqrMagnitude < 0.0001f) 
            return Vector3.zero;

        if (movementReference == null)
            return new Vector3(input.x, 0f, input.y).normalized;

        Vector3 forward = Vector3.ProjectOnPlane(movementReference.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(movementReference.right, Vector3.up).normalized;

        Vector3 direction = right * input.x + forward * input.y;
        return direction.normalized;
    }
    
    public Vector2 MoveInput => moveInput;

    public Vector3 CurrentVelocity
    {
        get
        {
            if (rigidbodyComponent == null)
                return Vector3.zero;

            return rigidbodyComponent.linearVelocity;
        }
    }

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
