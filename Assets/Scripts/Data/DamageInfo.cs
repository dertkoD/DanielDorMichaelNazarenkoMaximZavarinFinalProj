using System;
using UnityEngine;

[Serializable]
public struct DamageInfo
{
    public int sourceObjectId;
    public int targetObjectId;
    public int damage;

    public int hpBefore;
    public int hpAfter;

    public Vector3 hitPoint;
    public Vector3 hitNormal;
}
