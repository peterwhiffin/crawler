using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    private List<RaycastHit2D> m_TempHits = new();
    private int m_LegIndex;
    private Vector3 m_BestHit;
    private Vector2 m_MoveInput;

    [SerializeField] private CrawlerSettings m_CrawlerSettings;
    [SerializeField] private Rigidbody2D m_RigidBody;
    [SerializeField] private Transform m_RestPosition;
    [SerializeField] private List<Leg> m_Legs = new List<Leg>();  
    [SerializeField] private LayerMask m_LegLayerMask;
    
    public CrawlerSettings CrawlerSettings { get { return m_CrawlerSettings; } }

    private void Update()
    {
        SetRestPosition();
        MoveLegs();      
    }

    private void MoveLegs()
    {
        if (Vector3.Distance(transform.position, m_RestPosition.position) < m_CrawlerSettings.LegMoveThreshold ||
            !m_Legs[m_LegIndex].IsDoneMoving())
        {
            return;
        }

        int nextIndex = m_LegIndex + 1;
        if (nextIndex == m_Legs.Count)
        {
            nextIndex = 0;
        }

        if (!GetHits(m_Legs[nextIndex].m_Orientation.horizontal, m_Legs[nextIndex].m_Orientation.vertical) ||
            Vector3.Distance(m_Legs[nextIndex].transform.position, m_BestHit) < m_CrawlerSettings.MinimumLegMoveDistance)
        {
            return;
        }

        m_LegIndex = nextIndex;
        m_Legs[m_LegIndex].m_targetPosition.position = m_BestHit;
        m_Legs[m_LegIndex].StartMove();
    }

    private void SetRestPosition()
    {
        Vector3 restPosition = transform.position;

        foreach (Leg leg in m_Legs)
        {
            restPosition += leg.m_targetPosition.position;
        }

        restPosition /= m_Legs.Count + 1;
        m_RestPosition.position = restPosition;
    }

    private bool GetHits(float x, float y)
    {
        bool hitFound = false;
        float farthestHit = -1f;
        Vector3 origin = transform.position;
        Vector3 forward = origin + (m_CrawlerSettings.LegReach * x * transform.right);
        Vector3 up = origin + (m_CrawlerSettings.LegReach * y * transform.up);
        Vector3 max = origin + (m_CrawlerSettings.LegReach * x * transform.right) + (m_CrawlerSettings.LegReach * y * transform.up);

        m_TempHits.Add(Physics2D.Raycast(forward, max - forward, Vector3.Distance(max, forward), m_LegLayerMask));
        m_TempHits.Add(Physics2D.Raycast(up, max - up, Vector3.Distance(max, up), m_LegLayerMask));
        m_TempHits.Add(Physics2D.Raycast(origin, max - origin, Vector3.Distance(origin, max), m_LegLayerMask));
        m_TempHits.Add(Physics2D.Raycast(max, forward - max, Vector3.Distance(max, forward), m_LegLayerMask));       

        foreach (var hit in m_TempHits)
        {
            if (!hit || hit.fraction == 0f)
            {
                continue;
            }

            float distance = Vector3.Distance(hit.point, origin);

            if (distance > farthestHit)
            {
                farthestHit = distance;
                m_BestHit = hit.point;
                hitFound = true;
            }
        }

        if (!hitFound)
        {
            float closest = 100f;
            Vector3 halfY = origin + (m_CrawlerSettings.LegReach / 2f * y * transform.up);
            Vector3 halfX = origin + (m_CrawlerSettings.LegReach / 2f * x * transform.right);
            Vector3 reverseY = origin + (m_CrawlerSettings.LegReach * -y * transform.up);
            Vector3 reverseX = origin + (m_CrawlerSettings.LegReach * -x * transform.right);
            Vector3 reverseXMax = origin + (m_CrawlerSettings.LegReach * -x * transform.right) + (m_CrawlerSettings.LegReach * y * transform.up);
            Vector3 reverseYMax = origin + (m_CrawlerSettings.LegReach * x * transform.right) + (m_CrawlerSettings.LegReach * -y * transform.up);

            m_TempHits.Add(Physics2D.Raycast(max, reverseYMax - max, Vector3.Distance(max, reverseYMax), m_LegLayerMask));
            m_TempHits.Add(Physics2D.Raycast(up, reverseXMax - up, Vector3.Distance(up, reverseXMax), m_LegLayerMask));
            m_TempHits.Add(Physics2D.Raycast(halfY, reverseY - halfY, Vector3.Distance(reverseY, halfY), m_LegLayerMask));
            m_TempHits.Add(Physics2D.Raycast(halfX, reverseX - halfX, Vector3.Distance(reverseX, halfX), m_LegLayerMask));          

            foreach (var hit in m_TempHits)
            {
                if (!hit || hit.fraction == 0f)
                {
                    continue;
                }

                float distance = Vector3.Distance(hit.point, origin);

                if (distance < closest)
                {
                    closest = distance;
                    m_BestHit = hit.point;
                    hitFound = true;
                }
            }
        }

        m_TempHits.Clear();

        return hitFound;
    }

    private void FixedUpdate()
    {
        Vector3 direction = (m_RestPosition.position - transform.position).normalized;

        if ((transform.position - m_RestPosition.position).magnitude < m_CrawlerSettings.MaxSpringStretch)
        {
            if (m_MoveInput.x != 0f)
            {
                direction.x = 0f;
            }

            if (m_MoveInput.y != 0f)
            {
                direction.y = 0f;
            }
        }

        float offset = Vector3.Distance(m_RestPosition.position, transform.position);        
        float velocity = Vector3.Dot(direction, m_RigidBody.linearVelocity);
        float force = (offset * m_CrawlerSettings.SpringForce) - (velocity * m_CrawlerSettings.DampForce);
        Vector3 springForce = direction * force;

        Vector3 moveForce = m_CrawlerSettings.MoveSpeed * Time.fixedDeltaTime * m_MoveInput.normalized;
        m_RigidBody.AddForce(springForce + moveForce);
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        m_MoveInput = context.ReadValue<Vector2>();
    }
}
