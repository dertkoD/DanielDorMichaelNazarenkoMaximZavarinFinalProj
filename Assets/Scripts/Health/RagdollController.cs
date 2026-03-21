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

    [Header("Pose Detection")]
    [SerializeField] private Transform hipsBone;
    [SerializeField] private Transform chestBone;
    [SerializeField] private bool invertFaceUpCheck = false;

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
            SetRagdollState(true);

        canRecover = true;
        recoverAtTime = Mathf.Max(recoverAtTime, Time.time + Mathf.Max(minRagdollTime, extraTime));
    }

    public void HoldRagdoll(float extraTime)
    {
        if (!IsRagdollActive)
        {
            ActivateRagdoll(extraTime);
            return;
        }

        canRecover = true;
        recoverAtTime = Mathf.Max(recoverAtTime, Time.time + Mathf.Max(minRagdollTime, extraTime));
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

            if (faceUp)
            {
                if (!string.IsNullOrEmpty(getUpFromBackTrigger))
                    animator.SetTrigger(getUpFromBackTrigger);
            }
            else
            {
                if (!string.IsNullOrEmpty(getUpFromFrontTrigger))
                    animator.SetTrigger(getUpFromFrontTrigger);
            }
        }

        recoverAtTime = -1f;
        canRecover = false;
    }

    private Vector3 GetRagdollHipsPosition()
    {
        if (hipsBone != null)
            return new Vector3(hipsBone.position.x, transform.position.y, hipsBone.position.z);

        if (ragdollBodies != null && ragdollBodies.Length > 0 && ragdollBodies[0] != null)
            return new Vector3(ragdollBodies[0].position.x, transform.position.y, ragdollBodies[0].position.z);

        return transform.position;
    }

    private Quaternion GetStandUpRotation()
    {
        Transform referenceBone = chestBone != null ? chestBone : hipsBone;

        Vector3 forward = referenceBone != null ? referenceBone.forward : (visualRoot != null ? visualRoot.forward : transform.forward);
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f)
            forward = transform.forward;

        return Quaternion.LookRotation(forward.normalized, Vector3.up);
    }

    private bool IsFaceUp()
    {
        bool? chestResult = EvaluateBoneFaceUp(chestBone);
        bool? hipsResult = EvaluateBoneFaceUp(hipsBone);

        bool result;

        if (chestResult.HasValue)
            result = chestResult.Value;
        else if (hipsResult.HasValue)
            result = hipsResult.Value;
        else if (visualRoot != null)
            result = Vector3.Dot(visualRoot.up, Vector3.up) > 0f;
        else
            result = Vector3.Dot(transform.up, Vector3.up) > 0f;

        if (invertFaceUpCheck)
            result = !result;

        return result;
    }

    private bool? EvaluateBoneFaceUp(Transform bone)
    {
        if (bone == null)
            return null;

        float forwardDot = Vector3.Dot(bone.forward, Vector3.up);
        float upDot = Vector3.Dot(bone.up, Vector3.up);

        if (Mathf.Abs(forwardDot) >= Mathf.Abs(upDot))
            return forwardDot < 0f;

        return upDot > 0f;
    }
}
