using System;
using UnityEngine;

[Serializable] 
public struct WeaponPickupData
{
    public WeaponPickupEventType eventType;

    public Collider pickerCollider;
    public WeaponPickupTrigger pickupTrigger;

    public int pickerObjectId;
    public int weaponId;
    public Weapon weaponPrefab;
}
