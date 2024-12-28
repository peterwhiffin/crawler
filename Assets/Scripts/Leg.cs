using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class Leg : MonoBehaviour
{
    private float m_TimeElapsed;    
    //private bool m_DoMove;
    private bool m_IsLaunching = false;
    private bool m_NeedNewPosition = false;
    private bool m_IsDoneMoving = false;
    private List<RaycastHit2D> m_TempHits = new();
    private RaycastHit2D m_BestHit;

    [SerializeField] private Player m_Player;
    [SerializeField] private Transform m_TargetPosition;
    [SerializeField] private Vector2 m_Orientation;
    [SerializeField] private CrawlerSettings m_CrawlerSettings;
    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] private Rigidbody2D m_RigidBody;

    private List<Vector3> allRopeSections = new();
    public Vector2 Orientation { get { return m_Orientation; } }
    public Vector2 TargetPosition { get { return m_TargetPosition.position; } }
    public bool NeedsNewPosition { get { return m_NeedNewPosition; } }

    public void Initialize(float moveRate)
    {
        m_TimeElapsed = 0f;
        m_IsDoneMoving = false;
    }

    private void Update()
    {
        MoveTowardTarget();
    }

    private void FixedUpdate()
    {
        if (!m_IsLaunching)
        {
            return;
        }

        m_RigidBody.AddForce(GetSpringForce());
    }

    private void LateUpdate()
    {
        DisplayRope();
    }

    private void MoveTowardTarget()
    {
        if (m_IsLaunching || m_IsDoneMoving)
        {
            return;
        }

        m_IsDoneMoving = m_TimeElapsed >= m_CrawlerSettings.LegMoveRate;

        if (m_NeedNewPosition)
        {
            m_NeedNewPosition = false;
            FreezeConstraints();
        }

        float time = m_TimeElapsed / m_CrawlerSettings.LegMoveRate;
        Vector3 target = m_TargetPosition.position + m_TargetPosition.up * m_CrawlerSettings.StepHeight * m_CrawlerSettings.StepCurve.Evaluate(time);
        transform.position = Vector3.Lerp(transform.position, target, time);
        m_TimeElapsed += Time.deltaTime;
    }

    private void FreezeConstraints()
    {
        m_RigidBody.bodyType = RigidbodyType2D.Kinematic;
        m_RigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private Vector2 GetSpringForce()
    {
        Vector2 direction = m_RigidBody.position - (Vector2)m_Player.transform.position;
        float offset = m_CrawlerSettings.LegReach - direction.magnitude;
        float velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
        float force = (offset * m_CrawlerSettings.LegLaunchSpringStrength) - (velocity * m_CrawlerSettings.LegLaunchDamperStrength);
        return direction.normalized * force;
    }

    public void PlayerLaunched()
    {
        m_IsLaunching = true;
        m_RigidBody.bodyType = RigidbodyType2D.Dynamic;
        m_RigidBody.constraints = RigidbodyConstraints2D.None;
        m_NeedNewPosition = true;
        m_IsDoneMoving = true;
        m_TimeElapsed = 0f;
    }

    public void PlayerLanded()
    {      
        m_IsLaunching = false;
    }

    public void NewTarget()
    {
        m_TargetPosition.SetParent(null);
        m_TargetPosition.up = m_BestHit.normal;
        m_TargetPosition.SetParent(m_BestHit.transform);
        m_TargetPosition.position = m_BestHit.point;      
        transform.up = m_BestHit.normal;
        m_TimeElapsed = 0f;
        m_IsDoneMoving = false;
    }

    public bool IsDoneMoving()
    {
        return m_IsDoneMoving || m_NeedNewPosition;
    }

    public void SnapPosition(Vector3 position)
    {
        transform.position = position;
    }

    private void DisplayRope()
    {
        float ropeWidth = 0.1f;

        m_LineRenderer.startWidth = ropeWidth;
        m_LineRenderer.endWidth = ropeWidth;

        Vector3[] pos = new Vector3[2] { transform.position, m_Player.transform.position };
        m_LineRenderer.positionCount = 2;
        m_LineRenderer.SetPositions(pos);
    }

    private bool FindBestHit()
    {
        Vector3 origin = transform.position;
        Vector3 forward = origin + (m_CrawlerSettings.LegReach * m_Orientation.x * transform.right);
        Vector3 up = origin + (m_CrawlerSettings.LegReach * m_Orientation.y * transform.up);
        Vector3 max = origin + (m_CrawlerSettings.LegReach * m_Orientation.x * transform.right) + (m_CrawlerSettings.LegReach * m_Orientation.y * transform.up);
        Vector3 halfY = origin + (m_CrawlerSettings.LegReach / 2f * m_Orientation.y * transform.up);
        Vector3 halfX = origin + (m_CrawlerSettings.LegReach / 2f * m_Orientation.x * transform.right);
        Vector3 reverseY = origin + (m_CrawlerSettings.LegReach * -m_Orientation.y * transform.up);
        Vector3 reverseX = origin + (m_CrawlerSettings.LegReach * -m_Orientation.x * transform.right);

        RaycastHit2D originToMax = Physics2D.Raycast(origin, max - origin, Vector3.Distance(origin, max), m_CrawlerSettings.LegHitMask);
        RaycastHit2D forwardToMax = Physics2D.Raycast(forward, max - forward, Vector3.Distance(forward, max), m_CrawlerSettings.LegHitMask);
        RaycastHit2D upToMax = Physics2D.Raycast(up, max - up, Vector3.Distance(up, max), m_CrawlerSettings.LegHitMask);
        RaycastHit2D originToForward = Physics2D.Raycast(origin, forward - origin, Vector3.Distance(origin, forward), m_CrawlerSettings.LegHitMask);
        RaycastHit2D originToUp = Physics2D.Raycast(origin, up - origin, Vector3.Distance(up, origin), m_CrawlerSettings.LegHitMask);

        m_TempHits.Clear();

        if (!forwardToMax)
        {
            m_TempHits.Add(Physics2D.Raycast(max, forward - max, Vector3.Distance(max, forward), m_CrawlerSettings.LegHitMask));
        }

        if (!upToMax)
        {
            m_TempHits.Add(Physics2D.Raycast(max, up - max, Vector3.Distance(max, up), m_CrawlerSettings.LegHitMask));
        }


        if (FindFarthestHit())
            return true;

        m_TempHits.Clear();

        if (!originToForward)
        {
            m_TempHits.Add(forwardToMax);
        }

        if (!originToUp)
        {
            m_TempHits.Add(upToMax);
        }

        if (FindFarthestHit())
            return true;

        m_TempHits.Clear();
        if (!originToMax)
        {
            m_TempHits.Add(Physics2D.Raycast(max, origin - max, Vector3.Distance(max, origin), m_CrawlerSettings.LegHitMask));
        }

        if (FindFarthestHit())
            return true;

        m_TempHits.Clear();
        m_TempHits.Add(originToMax);

        if (FindFarthestHit())
            return true;

        m_TempHits.Clear();
        m_TempHits.Add(Physics2D.Raycast(halfX, reverseY - halfX, Vector3.Distance(halfX, reverseY), m_CrawlerSettings.LegHitMask));
        m_TempHits.Add(Physics2D.Raycast(halfY, reverseX - halfY, Vector3.Distance(halfY, reverseX), m_CrawlerSettings.LegHitMask));

        return FindFarthestHit();
    }

    private bool FindFarthestHit()
    {
        bool hitFound = false;
        float farthestHit = -1f;

        foreach (var hit in m_TempHits)
        {
            if (!hit)
            {
                continue;
            }

            float distance = Vector3.Distance(hit.point, transform.position);

            if (distance > farthestHit)
            {
                farthestHit = distance;
                m_BestHit = hit;
                hitFound = true;
            }
        }

        return hitFound;
    }

    public void LaunchDone()
    {
        if (FindBestHit())
        {
            NewTarget();
        }
    }
}

