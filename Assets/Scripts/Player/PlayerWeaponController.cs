using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private int objectId;

    [Header("References")]
    [SerializeField] private Collider playerBodyCollider;
    [SerializeField] private Transform weaponSocket;
    [SerializeField] private PlayerAimController playerAimController;

    [Header("Channels")]
    [SerializeField] private WeaponPickedEventChannelSO weaponPickedChannel;
    [SerializeField] private WeaponEquippedActionChannelSO weaponEquippedChannel;

    private Weapon currentWeapon;

    public bool HasWeapon => currentWeapon != null;
    public Weapon CurrentWeapon => currentWeapon;

    private void OnEnable()
    {
        if (weaponPickedChannel != null)
            weaponPickedChannel.Register(OnWeaponPickupEvent);
    }

    private void OnDisable()
    {
        if (weaponPickedChannel != null)
            weaponPickedChannel.Unregister(OnWeaponPickupEvent);
    }

    private void Update()
    {
        if (currentWeapon == null) return;
        if (playerAimController == null) return;
        if (!playerAimController.HasAimPoint) return;

        currentWeapon.SetAimPoint(playerAimController.CurrentAimPoint);
    }

    private void OnWeaponPickupEvent(WeaponPickupData data)
    {
        if (data.eventType == WeaponPickupEventType.Entered)
        {
            HandlePickupEntered(data);
            return;
        }

        if (data.eventType == WeaponPickupEventType.Picked)
        {
            HandlePickupConfirmed(data);
        }
    }

    private void HandlePickupEntered(WeaponPickupData data)
    {
        if (playerBodyCollider == null) return;
        if (data.pickupTrigger == null) return;
        if (data.pickerCollider != playerBodyCollider) return;

        data.pickupTrigger.TryConsume(objectId);
    }

    private void HandlePickupConfirmed(WeaponPickupData data)
    {
        if (data.pickerObjectId != objectId) return;
        if (data.weaponPrefab == null) return;
        if (weaponSocket == null) return;

        EquipWeapon(data.weaponPrefab, data.weaponId);
    }

    private void EquipWeapon(Weapon weaponPrefab, int weaponId)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);

        currentWeapon = Instantiate(weaponPrefab, weaponSocket);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        weaponEquippedChannel?.Raise(objectId, weaponId);
    }

    public void StartFire()
    {
        if (!HasWeapon) return;
        currentWeapon.StartFire();
    }

    public void StopFire()
    {
        if (!HasWeapon) return;
        currentWeapon.StopFire();
    }

    public void TryFireOnce()
    {
        if (!HasWeapon) return;
        currentWeapon.TryFireOnce();
    }
}
