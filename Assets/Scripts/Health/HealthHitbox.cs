using UnityEngine;

public class HealthHitbox : MonoBehaviour
{
    [SerializeField] private Collider targetCollider;
    [SerializeField] private Health health;
    [SerializeField] private float damageMultiplier = 1f;

    public Health Health => health;
    public int ObjectId => health != null ? health.ObjectId : -1;
    public float DamageMultiplier => damageMultiplier;

    private void OnEnable()
    {
        if (targetCollider != null)
            HealthHitboxRegistry.Register(targetCollider, this);
    }

    private void OnDisable()
    {
        if (targetCollider != null)
            HealthHitboxRegistry.Unregister(targetCollider, this);
    }
}
