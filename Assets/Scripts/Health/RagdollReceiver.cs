using UnityEngine;

public class RagdollReceiver : MonoBehaviour
{
    [SerializeField] private RagdollController ragdollController;

    public void NotifyRagdollContact(float holdTime)
    {
        if (ragdollController == null) return;
        ragdollController.HoldRagdoll(holdTime);
    } 
}
