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

    [Header("Inertia / Ice")]
    [SerializeField] private float groundSlideDecay = 2.5f;
    [SerializeField] private float airSlideDecay = 0.5f;

    [Header("Dirt")]
    [SerializeField] private string dirtLayerName = "Dirt";
    [SerializeField] private float dirtSpeedMultiplier = 0.45f;
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
        if (!rigidbodyComponent)
            return;

        bool isGrounded = groundChecker && groundChecker.IsGrounded;

        Vector3 velocity = rigidbodyComponent.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        float verticalVelocity = velocity.y;

        float currentGroundSpeed = groundSpeed;
        float currentGroundSlideDecay = groundSlideDecay;

        if (IsOnDirtSurface())
        {
            currentGroundSpeed *= dirtSpeedMultiplier;
            currentGroundSlideDecay *= dirtSlideDecayMultiplier;
        }

        float targetSpeed = isGrounded ? currentGroundSpeed : airSpeed;
        Vector3 moveDirection = GetMoveDirection(moveInput);

        Vector3 newHorizontalVelocity;

        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            newHorizontalVelocity = moveDirection * targetSpeed;
        }
        else
        {
            float decay = isGrounded ? currentGroundSlideDecay : airSlideDecay;

            newHorizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity,
                Vector3.zero,
                decay * Time.fixedDeltaTime
            );
        }

        Vector3 finalVelocity = new Vector3(
            newHorizontalVelocity.x,
            verticalVelocity,
            newHorizontalVelocity.z
        );

        if (isGrounded && finalVelocity.y <= 0f)
            finalVelocity.y -= groundedDownForce * Time.fixedDeltaTime;

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

    private Vector3 GetMoveDirection(Vector2 input)
    {
        if (input.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        Vector3 direction = new Vector3(input.x, 0f, input.y);
        return Vector3.ClampMagnitude(direction, 1f);
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