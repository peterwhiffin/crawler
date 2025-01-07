using UnityEngine;

public interface IHittable
{
    public void Hit(float damage, Vector3 hitPosition, Vector3 hitNormal) { }
}
