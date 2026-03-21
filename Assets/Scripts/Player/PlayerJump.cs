using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbodyComponent;
    [SerializeField] private GroundChecker groundChecker;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpCooldown = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private string noJumpLayerName = "Dirt";

    [Header("Falling")]
    [SerializeField] private float extraFallGravity = 12f;
    [SerializeField] private float lowJumpGravity = 8f;

    private bool jumpHeld;
    private bool jumpQueued;
    private float lastJumpPressedTime = -999f;
    private float lastJumpTime = -999f;

    private bool IsOnNoJumpSurface()
    {
        if (!groundChecker.IsGrounded)
            return false;

        Collider ground = groundChecker.GroundCollider;
        if (!ground)
            return false;
        
        return ground.gameObject.layer == LayerMask.NameToLayer(noJumpLayerName);
    }
    
    public void QueueJump()
    {
        jumpQueued = true;
        lastJumpPressedTime = Time.time;
    }

    public void SetJumpHeld(bool value)
    {
        jumpHeld = value;
    }

    private void FixedUpdate()
    {
        ApplyExtraGravity();
        TryJump();
    }

    private void TryJump()
    {
        if (rigidbodyComponent == null || groundChecker == null)
            return;

        bool hasBufferedJump = jumpQueued && Time.time - lastJumpPressedTime <= jumpBufferTime;
        bool cooldownReady = Time.time - lastJumpTime >= jumpCooldown;

        if (!groundChecker.IsGrounded || !hasBufferedJump || !cooldownReady)
            return;
        
        if (IsOnNoJumpSurface())
            return;

        Vector3 velocity = rigidbodyComponent.linearVelocity;
        velocity.y = 0f;
        rigidbodyComponent.linearVelocity = velocity;

        rigidbodyComponent.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        jumpQueued = false;
        lastJumpTime = Time.time;
    }

    private void ApplyExtraGravity()
    {
        if (rigidbodyComponent == null)
            return;

        float verticalVelocity = rigidbodyComponent.linearVelocity.y;

        if (verticalVelocity < 0f)
        {
            rigidbodyComponent.AddForce(Vector3.down * extraFallGravity, ForceMode.Acceleration);
        }
        else if (verticalVelocity > 0f && !jumpHeld)
        {
            rigidbodyComponent.AddForce(Vector3.down * lowJumpGravity, ForceMode.Acceleration);
        }
    }
}
