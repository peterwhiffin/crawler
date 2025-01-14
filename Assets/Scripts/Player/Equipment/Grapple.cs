using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Grapple : Equipment
{
    public Transform m_FirePosition;
    public override Transform FirePosition {  get { return m_FirePosition; } }
    public float m_GrappleDistance;
    public LayerMask m_HitMask;
    public Action<Vector2, float> GrappleHit = delegate { };
    public event Action GrappleReleased = delegate { };
    public GrappleProjectile m_Projectile;
    public float m_Speed;
    public LineRenderer m_LineRenderer;
    public Vector3[] m_RopePositions;
    public Transform m_RopeTarget;
    public bool m_IsActive = false;
    public HitBox m_GrabbedObject;
    private Coroutine ReelRoutine;
    private bool m_IsReeling = false;
    private bool m_IsGrappled = false;

    private void Start()
    {
        m_LineRenderer.enabled = false;
        m_RopePositions = new Vector3[2];
    }

    public override bool StartPrimaryAttack()
    {
        base.StartPrimaryAttack();
        
        if (m_IsGrappled)
        {
            ReelGrapple();
            return false;
        }

        m_Projectile.gameObject.SetActive(true);
        m_LineRenderer.enabled = true;
        m_Projectile.Fire(m_FirePosition.position, m_FirePosition.rotation, m_Speed, 50f, .1f);
        m_IsActive = true;
        return false;
    }

    public void ReelGrapple()
    {
        m_IsGrappled = false;
        GrappleReleased.Invoke();
        if (ReelRoutine != null)
        {
            StopCoroutine(ReelRoutine);
        }

        ReelRoutine = StartCoroutine(ReelInProjectile());
    }

    public void ReelObjectIn(Vector2 position, IGrappleable grappleable)
    {

        m_IsReeling = true;
        GrappleHit.Invoke(position, .3f);
        if(ReelRoutine != null)
        {
            StopCoroutine(ReelRoutine);
        }

       ReelRoutine = StartCoroutine(ReelInObject(grappleable));
    }

    public void ReelPlayerIn(Vector2 position)
    {
        GrappleHit.Invoke(position, 1f);

        m_IsGrappled = true;


        
    }

    public void ProjectileMissed()
    {
        if (ReelRoutine != null)
        {
            StopCoroutine(ReelRoutine);
        }

        ReelRoutine = StartCoroutine(ReelInProjectile());
    }

    private IEnumerator ReelInProjectile()
    {
        float timeElapsed = 0f;

        while (Vector3.Distance(transform.position, m_Projectile.transform.position) > .2f)
        {
            m_Projectile.transform.position = Vector3.Lerp(m_Projectile.transform.position, transform.position, timeElapsed / .9f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        m_Projectile.gameObject.SetActive(false);
        m_IsReeling = false;
        m_IsActive = false;
        m_LineRenderer.enabled = false;
    }

    private IEnumerator ReelInObject(IGrappleable grappleable)
    {
        float timeElapsed = 0f;

        while (Vector3.Distance(transform.position, m_Projectile.transform.position) > .2f)
        {
            m_Projectile.transform.position = Vector3.Lerp(m_Projectile.transform.position, transform.position, timeElapsed / .9f);
            grappleable.PullTransform.position = m_Projectile.transform.position;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        grappleable.HasReeledIn();
        m_Projectile.gameObject.SetActive(false);
        m_IsReeling = false;
        m_IsActive = false;
        m_LineRenderer.enabled = false;
    }
}
