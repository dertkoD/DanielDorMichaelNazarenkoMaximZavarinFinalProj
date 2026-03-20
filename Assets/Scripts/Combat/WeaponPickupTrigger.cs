using UnityEngine;

public class WeaponPickupTrigger : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private int weaponId = 1;
    [SerializeField] private Weapon weaponPrefab;

    [Header("Filter")]
    [SerializeField] private LayerMask allowedPickerMask;

    [Header("Channel")]
    [SerializeField] private WeaponPickedEventChannelSO weaponPickedChannel;

    private bool consumed;

    public int WeaponId => weaponId;
    public Weapon WeaponPrefab => weaponPrefab;
    public bool IsConsumed => consumed;

    private void OnTriggerEnter(Collider other)
    {
        if (consumed) return;
        if (other.isTrigger) return;
        if (weaponPickedChannel == null) return;

        int bit = 1 << other.gameObject.layer;
        if ((allowedPickerMask.value & bit) == 0) return;

        weaponPickedChannel.Raise(new WeaponPickupData
        {
            eventType = WeaponPickupEventType.Entered,
            pickerCollider = other,
            pickupTrigger = this,
            weaponId = weaponId,
            weaponPrefab = weaponPrefab
        });
    }

    public bool TryConsume(int pickerObjectId)
    {
        if (consumed) return false;

        consumed = true;

        weaponPickedChannel?.Raise(new WeaponPickupData
        {
            eventType = WeaponPickupEventType.Picked,
            pickerObjectId = pickerObjectId,
            pickupTrigger = this,
            weaponId = weaponId,
            weaponPrefab = weaponPrefab
        });

        Destroy(gameObject);
        return true;
    }
}