using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] private PlayerWeaponController playerWeaponController;

    [Header("Animator Params")]
    [SerializeField] private string xVelocityParam = "xVelocity";
    [SerializeField] private string yVelocityParam = "yVelocity";
    [SerializeField] private string isGroundedParam = "IsGrounded";
    [SerializeField] private string verticalVelocityParam = "VerticalVelocity";
    [SerializeField] private string hasWeaponParam = "HasWeapon";
    [SerializeField] private string aimParam = "Aim";
    [SerializeField] private string reloadTriggerParam = "Reload";

    [Header("Tuning")]
    [SerializeField] private float maxAnimSpeed = 6f;
    [SerializeField] private float dampTime = 0.08f;

    private Weapon observedWeapon;
    private bool wasReloadingLastFrame;

    private void Update()
    {
        if (animator == null || visualRoot == null || playerMovement == null)
            return;

        UpdateMovementParams();
        UpdateWeaponParams();
    }

    private void UpdateMovementParams()
    {
        Vector3 worldVelocity = playerMovement.HorizontalVelocity;
        Vector3 localVelocity = visualRoot.InverseTransformDirection(worldVelocity);

        float x = Mathf.Clamp(localVelocity.x / maxAnimSpeed, -1f, 1f);
        float y = Mathf.Clamp(localVelocity.z / maxAnimSpeed, -1f, 1f);

        animator.SetFloat(xVelocityParam, x, dampTime, Time.deltaTime);
        animator.SetFloat(yVelocityParam, y, dampTime, Time.deltaTime);

        bool isGrounded = groundChecker != null && groundChecker.IsGrounded;
        animator.SetBool(isGroundedParam, isGrounded);

        animator.SetFloat(verticalVelocityParam, playerMovement.CurrentVelocity.y);
        animator.SetBool(aimParam, true);
    }

    private void UpdateWeaponParams()
    {
        bool hasWeapon = playerWeaponController != null && playerWeaponController.HasWeapon;
        animator.SetBool(hasWeaponParam, hasWeapon);

        Weapon currentWeapon = hasWeapon ? playerWeaponController.CurrentWeapon : null;

        if (currentWeapon != observedWeapon)
        {
            observedWeapon = currentWeapon;
            wasReloadingLastFrame = observedWeapon != null && observedWeapon.IsReloading;
            return;
        }

        if (observedWeapon == null)
        {
            wasReloadingLastFrame = false;
            return;
        }

        bool isReloadingNow = observedWeapon.IsReloading;

        if (!wasReloadingLastFrame && isReloadingNow)
        {
            if (!string.IsNullOrEmpty(reloadTriggerParam))
                animator.SetTrigger(reloadTriggerParam);
        }

        wasReloadingLastFrame = isReloadingNow;
    }
}
