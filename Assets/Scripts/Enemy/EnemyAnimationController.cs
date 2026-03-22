using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
   [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Health health;

    [Header("Animator Params")]
    [SerializeField] private string xVelocityParam = "xVelocity";
    [SerializeField] private string yVelocityParam = "yVelocity";
    [SerializeField] private string deathParam = "Death";

    [Header("Tuning")]
    [SerializeField] private float maxAnimSpeed = 4f;
    [SerializeField] private float dampTime = 0.08f;
    [SerializeField] private float minVelocityToAnimate = 0.03f;

    private bool deathSent;

    private void Update()
    {
        if (animator == null || visualRoot == null)
            return;

        UpdateMovementAnimation();
        UpdateDeathAnimation();
    }

    private void UpdateMovementAnimation()
    {
        Vector3 worldVelocity = GetWorldVelocity();
        worldVelocity.y = 0f;

        if (worldVelocity.magnitude < minVelocityToAnimate)
            worldVelocity = Vector3.zero;

        Vector3 localVelocity = visualRoot.InverseTransformDirection(worldVelocity);

        float x = 0f;
        float y = 0f;

        if (maxAnimSpeed > 0.0001f)
        {
            x = Mathf.Clamp(localVelocity.x / maxAnimSpeed, -1f, 1f);
            y = Mathf.Clamp(localVelocity.z / maxAnimSpeed, -1f, 1f);
        }

        animator.SetFloat(xVelocityParam, x, dampTime, Time.deltaTime);
        animator.SetFloat(yVelocityParam, y, dampTime, Time.deltaTime);
    }

    private Vector3 GetWorldVelocity()
    {
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            return navMeshAgent.velocity;
        }

        return Vector3.zero;
    }

    private void UpdateDeathAnimation()
    {
        if (health == null) return;
        if (!health.IsDead) return;
        if (deathSent) return;

        deathSent = true;
        if (!string.IsNullOrEmpty(deathParam))
            animator.SetTrigger(deathParam);
    }
}
