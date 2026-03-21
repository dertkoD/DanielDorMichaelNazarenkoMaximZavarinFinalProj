using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Action Channel/Health Changed")]
public class HealthChangedActionChannelSO : ScriptableObject
{
    public event Action<int, int, int> OnEvent; // objectId, currentHp, maxHp

    public void Raise(int objectId, int currentHp, int maxHp)
    {
        OnEvent?.Invoke(objectId, currentHp, maxHp);
    }
}
