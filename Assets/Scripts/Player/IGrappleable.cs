using UnityEngine;

public interface IGrappleable
{
    public Transform PullTransform { get; }
    public bool GrappleHit(float damage, Vector3 point, Vector3 normal, Transform reelTarget) { return false; }
    public void HasReeledIn() { }
}
