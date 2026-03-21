using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody mainRigidbody;
    [SerializeField] private Collider mainCollider;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Behaviour[] behavioursToDisable;

    [Header("Ragdoll")]
    [SerializeField] private Rigidbody[] ragdollBodies;
    [SerializeField] private Collider[] ragdollColliders;

    [Header("Recovery")]
    [SerializeField] private float minRagdollTime = 1.25f;
    [SerializeField] private string getUpFromBackTrigger = "GetUpFromBack";
    [SerializeField] private string getUpFromFrontTrigger = "GetUpFromFront";

    public bool IsRagdollActive { get; private set; }

    private float recoverAtTime = -1f;
    private bool canRecover;

    private void Awake()
    {
        SetRagdollState(false);
    }

    private void Update()
    {
        if (!IsRagdollActive) return;
        if (!canRecover) return;
        if (Time.time < recoverAtTime) return;

        RecoverFromRagdoll();
    }

    public void ActivateRagdoll(float extraTime)
    {
        if (!IsRagdollActive)
        {
            SetRagdollState(true);
        }

        canRecover = true;
        recoverAtTime = Mathf.Max(recoverAtTime, Time.time + extraTime);
    }

    public void HoldRagdoll(float extraTime)
    {
        if (!IsRagdollActive)
        {
            ActivateRagdoll(extraTime);
            return;
        }

        canRecover = true;
        recoverAtTime = Mathf.Max(recoverAtTime, Time.time + extraTime);
    }

    public void ForceRecoverNow()
    {
        if (!IsRagdollActive) return;
        RecoverFromRagdoll();
    }

    private void SetRagdollState(bool active)
    {
        IsRagdollActive = active;

        if (animator != null)
            animator.enabled = !active;

        if (mainCollider != null)
            mainCollider.enabled = !active;

        if (mainRigidbody != null)
        {
            mainRigidbody.isKinematic = active;
            mainRigidbody.linearVelocity = Vector3.zero;
            mainRigidbody.angularVelocity = Vector3.zero;
        }

        if (behavioursToDisable != null)
        {
            foreach (var behaviour in behavioursToDisable)
            {
                if (behaviour == null) continue;
                behaviour.enabled = !active;
            }
        }

        if (ragdollBodies != null)
        {
            foreach (var body in ragdollBodies)
            {
                if (body == null) continue;
                body.isKinematic = !active;
                if (!active)
                {
                    body.linearVelocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                }
            }
        }

        if (ragdollColliders != null)
        {
            foreach (var col in ragdollColliders)
            {
                if (col == null) continue;
                col.enabled = active;
            }
        }
    }

    private void RecoverFromRagdoll()
    {
        Vector3 hipsPosition = GetRagdollHipsPosition();
        Quaternion targetRotation = GetStandUpRotation();
        bool faceUp = IsFaceUp();

        transform.position = hipsPosition;
        transform.rotation = targetRotation;

        SetRagdollState(false);

        if (animator != null)
        {
            animator.enabled = true;
            animator.Play("Idle", 0, 0f);

            if (faceUp && !string.IsNullOrEmpty(getUpFromBackTrigger))
                animator.SetTrigger(getUpFromBackTrigger);
            else if (!faceUp && !string.IsNullOrEmpty(getUpFromFrontTrigger))
                animator.SetTrigger(getUpFromFrontTrigger);
        }

        recoverAtTime = -1f;
        canRecover = false;
    }

    private Vector3 GetRagdollHipsPosition()
    {
        if (ragdollBodies == null || ragdollBodies.Length == 0)
            return transform.position;

        Rigidbody hips = ragdollBodies[0];
        return new Vector3(hips.position.x, transform.position.y, hips.position.z);
    }

    private Quaternion GetStandUpRotation()
    {
        Vector3 forward = visualRoot != null ? visualRoot.forward : transform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
            forward = transform.forward;

        return Quaternion.LookRotation(forward.normalized, Vector3.up);
    }

    private bool IsFaceUp()
    {
        if (visualRoot == null)
            return Vector3.Dot(transform.up, Vector3.up) > 0f;

        return Vector3.Dot(visualRoot.up, Vector3.up) > 0f;
    }
}
