using UnityEngine;
using System;

public class Grapple : Equipment
{
    public Transform m_FirePosition;
    public override Transform FirePosition {  get { return m_FirePosition; } }
    public float m_GrappleDistance;
    public LayerMask m_HitMask;
    public Action<Vector2> GrappleHit = delegate { };

    public override bool StartPrimaryAttack()
    {
        base.StartPrimaryAttack();
        RaycastHit2D hit = Physics2D.Raycast(m_FirePosition.position, m_FirePosition.up, m_GrappleDistance, m_HitMask);

        if (hit)
        {
            GrappleHit.Invoke(hit.point);
        }

        return false;
    }
}
