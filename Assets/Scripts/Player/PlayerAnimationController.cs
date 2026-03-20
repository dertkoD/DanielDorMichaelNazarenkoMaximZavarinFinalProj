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
    [SerializeField] private string jumpParam = "Jump";
    [SerializeField] private string hasWeaponParam = "HasWeapon";
    [SerializeField] private string deathParam = "Death";
    [SerializeField] private string aimParam = "Aim";

    [Header("Speed Mapping")]
    [SerializeField] private float maxAnimSpeed = 6f;
    [SerializeField] private float dampTime = 0.08f;

    private void Update()
    {
        if (animator == null || visualRoot == null || playerMovement == null)
            return;

        Vector3 worldVelocity = playerMovement.HorizontalVelocity;
        Vector3 localVelocity = visualRoot.InverseTransformDirection(worldVelocity);

        float x = Mathf.Clamp(localVelocity.x / maxAnimSpeed, -1f, 1f);
        float y = Mathf.Clamp(localVelocity.z / maxAnimSpeed, -1f, 1f);

        animator.SetFloat(xVelocityParam, x, dampTime, Time.deltaTime);
        animator.SetFloat(yVelocityParam, y, dampTime, Time.deltaTime);

        animator.SetBool(hasWeaponParam, playerWeaponController != null && playerWeaponController.HasWeapon);
        animator.SetBool(aimParam, true);
    }
}
