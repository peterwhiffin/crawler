using UnityEngine;
using System;

public class HitBox : MonoBehaviour, IHittable
{
    public Action<float> OnHit = delegate { };

    public void Hit(float damage, Vector3 point, Vector3 normal)
    {
        OnHit.Invoke(damage);
    }
}
