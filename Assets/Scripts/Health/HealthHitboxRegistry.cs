using System.Collections.Generic;
using UnityEngine;

public static  class HealthHitboxRegistry
{
    private static readonly Dictionary<Collider, HealthHitbox> map = new();

    public static void Register(Collider collider, HealthHitbox hitbox)
    {
        if (collider == null || hitbox == null) return;
        map[collider] = hitbox;
    }

    public static void Unregister(Collider collider, HealthHitbox hitbox)
    {
        if (collider == null) return;

        if (map.TryGetValue(collider, out var current) && current == hitbox)
            map.Remove(collider);
    }

    public static bool TryGet(Collider collider, out HealthHitbox hitbox)
    {
        return map.TryGetValue(collider, out hitbox);
    } 
}
