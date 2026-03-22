using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Action Channel/Game Over")]
public class GameOverActionChannelSO : ScriptableObject
{
    public event Action<int> OnEvent; // deadObjectId
    public void Raise(int deadObjectId) => OnEvent?.Invoke(deadObjectId);
}
