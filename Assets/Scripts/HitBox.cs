using UnityEngine;
using System;
using System.Collections;

public class HitBox : MonoBehaviour, IHittable, IGrappleable
{
    public Action<float> OnHit = delegate { };
    public Transform PullTransform { get { return transform; } }
    public bool CanReelIn;
    public Action ReeledIn = delegate { };
    public Action OnGrappleHit = delegate { };
    public void Hit(float damage, Vector3 point, Vector3 normal)
    {
        OnHit.Invoke(damage);
    }

    public bool GrappleHit(float damage, Vector3 point, Vector3 normal, Transform reelTarget)
    {
        //OnHit.Invoke(damage);
        OnGrappleHit.Invoke();
        return CanReelIn;
    }

    public void HasReeledIn()
    {
        ReeledIn.Invoke();
    }
}
