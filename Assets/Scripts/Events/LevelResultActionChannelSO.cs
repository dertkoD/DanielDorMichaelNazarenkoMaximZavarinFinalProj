using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelResultActionChannelSO", menuName = "Scriptable Objects/LevelResultActionChannelSO")]
public class LevelResultActionChannelSO : ScriptableObject
{
    public event Action<bool> OnEvent; // isVictory
    public void Raise(bool isVictory) => OnEvent?.Invoke(isVictory);
}
