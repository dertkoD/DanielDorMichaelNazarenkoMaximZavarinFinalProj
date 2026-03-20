using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerJump playerJump;
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] private PlayerWeaponController playerWeaponController;

    [Header("Animator Params")]
    [SerializeField] private string xVelocityParam = "xVelocity";
    [SerializeField] private string yVelocityParam = "yVelocity";
    [SerializeField] private string isGroundedParam = "IsGrounded";
    [SerializeField] private string hasWeaponParam = "HasWeapon";
    [SerializeField] private string verticalVelocityParam = "VerticalVelocity";

    [Header("Tuning")]
    [SerializeField] private float velocityNormalization = 6f;
    [SerializeField] private float dampTime = 0.08f;

    private void Update()
    {
        if (animator == null || visualRoot == null || playerMovement == null)
            return;

        Vector3 worldVelocity = playerMovement.HorizontalVelocity;
        Vector3 localVelocity = visualRoot.InverseTransformDirection(worldVelocity);

        float x = Mathf.Clamp(localVelocity.x / velocityNormalization, -1f, 1f);
        float y = Mathf.Clamp(localVelocity.z / velocityNormalization, -1f, 1f);

        animator.SetFloat(xVelocityParam, x, dampTime, Time.deltaTime);
        animator.SetFloat(yVelocityParam, y, dampTime, Time.deltaTime);

        bool isGrounded = groundChecker != null && groundChecker.IsGrounded;
        bool hasWeapon = playerWeaponController != null && playerWeaponController.HasWeapon;

        animator.SetBool(isGroundedParam, isGrounded);
        animator.SetBool(hasWeaponParam, hasWeapon);
        animator.SetFloat(verticalVelocityParam, playerMovement.CurrentVelocity.y);
    }

    public void PlayJumpStart()
    {
        animator.SetTrigger("JumpTrigger");
    }
}
