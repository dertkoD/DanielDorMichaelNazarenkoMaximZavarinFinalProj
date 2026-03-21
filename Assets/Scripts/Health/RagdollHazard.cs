using UnityEngine;

public class RagdollHazard : MonoBehaviour
{
    [SerializeField] private LayerMask affectedLayers;
    [SerializeField] private float ragdollHoldTime = 1.25f;

    private void OnTriggerEnter(Collider other)
    {
        TryActivate(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryActivate(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryActivate(collision.collider);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryActivate(collision.collider);
    }

    private void TryActivate(Collider other)
    {
        if (other == null) return;
        if (((1 << other.gameObject.layer) & affectedLayers.value) == 0) return;

        RagdollReceiver receiver = other.GetComponentInParent<RagdollReceiver>();
        if (receiver == null) return;

        receiver.NotifyRagdollContact(ragdollHoldTime);
    } 
}
