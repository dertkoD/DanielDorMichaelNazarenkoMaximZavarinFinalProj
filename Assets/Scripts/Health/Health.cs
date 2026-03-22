using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private int objectId;

    [Header("Stats")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int currentHp = 100;

    [Header("UI / Events")]
    [SerializeField] private bool raiseHealthOnEnable = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTrigger = "Death";
    [SerializeField] private string hasWeaponParam = "HasWeapon";

    [Header("Disable On Death")]
    [SerializeField] private Behaviour[] behavioursToDisable;
    [SerializeField] private Collider[] collidersToDisable;
    [SerializeField] private Rigidbody rigidbodyToStop;

    [Header("Events In")]
    [SerializeField] private DamageEventChannelSO damageEventChannel;

    [Header("Events Out")]
    [SerializeField] private HealthChangedActionChannelSO healthChangedAction;
    [SerializeField] private GameOverActionChannelSO gameOverAction;

    public int ObjectId => objectId;
    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public bool IsDead => currentHp <= 0;

    private bool deathTriggered;
    private bool gameOverRaised;

    private void Awake()
    {
        if (maxHp < 1)
            maxHp = 1;

        if (currentHp <= 0 || currentHp > maxHp)
            currentHp = maxHp;
    }

    private void OnEnable()
    {
        damageEventChannel?.Register(OnDamageReceived);

        deathTriggered = false;
        gameOverRaised = false;
    }

    private void Start()
    {
        if (raiseHealthOnEnable)
            RaiseHealthChanged();
    }

    private void OnDisable()
    {
        damageEventChannel?.Unregister(OnDamageReceived);
    }

    private void OnDamageReceived(DamageInfo info)
    {
        if (IsDead) return;
        if (info.targetObjectId != objectId) return;
        if (info.damage <= 0) return;

        currentHp = Mathf.Max(0, currentHp - info.damage);
        RaiseHealthChanged();

        if (!IsDead) return;

        TriggerDeathAnimation();
        DisableDeadComponents();

        if (!gameOverRaised && gameOverAction != null)
        {
            gameOverRaised = true;
            gameOverAction.Raise(objectId);
        }
    }

    private void RaiseHealthChanged()
    {
        healthChangedAction?.Raise(objectId, currentHp, maxHp);
    }

    private void TriggerDeathAnimation()
    {
        if (deathTriggered) return;
        deathTriggered = true;

        if (animator == null) return;

        if (!string.IsNullOrEmpty(hasWeaponParam))
            animator.SetBool(hasWeaponParam, false);

        if (!string.IsNullOrEmpty(deathTrigger))
            animator.SetTrigger(deathTrigger);
    }

    private void DisableDeadComponents()
    {
        if (behavioursToDisable != null)
        {
            foreach (var behaviour in behavioursToDisable)
            {
                if (behaviour == null) continue;
                behaviour.enabled = false;
            }
        }

        if (collidersToDisable != null)
        {
            foreach (var col in collidersToDisable)
            {
                if (col == null) continue;
                col.enabled = false;
            }
        }

        if (rigidbodyToStop != null)
        {
            rigidbodyToStop.linearVelocity = Vector3.zero;
            rigidbodyToStop.angularVelocity = Vector3.zero;
        }
    }

    public void HealToFull()
    {
        currentHp = maxHp;
        deathTriggered = false;
        gameOverRaised = false;
        RaiseHealthChanged();
    }
}
