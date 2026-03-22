using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Action Channel/Health Changed")]
public class HealthChangedActionChannelSO : ScriptableObject
{
    public event Action<int, int, int> OnEvent; // objectId, currentHp, maxHp

    public void Raise(int objectId, int currentHp, int maxHp)
    {
        Debug.Log($"HealthChangedActionChannelSO Raise: objectId={objectId}, hp={currentHp}/{maxHp}, listeners={(OnEvent == null ? 0 : OnEvent.GetInvocationList().Length)}");
        OnEvent?.Invoke(objectId, currentHp, maxHp);
    }
}
