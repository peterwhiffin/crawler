using UnityEngine;
using System;

public class HitBox : MonoBehaviour, IHittable
{
    public Action<float> OnHit = delegate { };

    public void Hit(float damage)
    {
        OnHit.Invoke(damage);
    }
}
