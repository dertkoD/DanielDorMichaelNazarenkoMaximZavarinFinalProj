using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Action Channel/Weapon Equipped")]
public class WeaponEquippedActionChannelSO : ScriptableObject
{
    public event Action<int, int> OnEvent; // objectId, weaponId
    public void Raise(int objectId, int weaponId) => OnEvent?.Invoke(objectId, weaponId);
}
