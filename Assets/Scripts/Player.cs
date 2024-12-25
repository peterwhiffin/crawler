using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using System.Threading.Tasks;

public class Player : MonoBehaviour
{
    private List<RaycastHit2D> m_TempHits = new();
    private RaycastHit2D m_BestHit;
    private Vector2 m_MoveInput;
    private int m_LegIndex;

    [SerializeField] private CrawlerSettings m_CrawlerSettings;
    [SerializeField] private Rigidbody2D m_RigidBody;
    [SerializeField] private Transform m_RestPosition;
    [SerializeField] private List<Leg> m_Legs = new();  
    [SerializeField] private LayerMask m_LegLayerMask;
    
    public CrawlerSettings CrawlerSettings { get { return m_CrawlerSettings; } }

    private void Start()
    {
        foreach(Leg leg in m_Legs)
        {
            leg.Initialize(m_CrawlerSettings.LegMoveRate);
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        m_MoveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        SetRestPosition();
        MoveLegs();      
    }

    private void FixedUpdate()
    {
        Vector2 springForce = Vector2.zero;
        Vector2 direction = (m_RestPosition.position - transform.position).normalized;
        float offset = Vector2.Distance(m_RestPosition.position, transform.position);
        float velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
        float force = (offset * m_CrawlerSettings.SpringForce) - (velocity * m_CrawlerSettings.DampForce);
        springForce = direction * force;

        if (m_MoveInput == Vector2.zero)
        {
            direction = (m_RestPosition.position - transform.position).normalized;
            offset = Vector2.Distance(m_RestPosition.position, transform.position);
            velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
            force = (offset * m_CrawlerSettings.SpringForce) - (velocity * m_CrawlerSettings.DampForce);
            springForce = direction * force;
        }       
        else if (Vector3.Distance(transform.position, m_RestPosition.position) > m_CrawlerSettings.MaxSpringStretch - .1f)
        {
            Vector3 pos = m_RestPosition.position + (transform.position - m_RestPosition.position).normalized * m_CrawlerSettings.MaxSpringStretch;
            direction = (pos - transform.position).normalized;
            offset = m_CrawlerSettings.MaxSpringStretch - Vector2.Distance(m_RestPosition.position, transform.position);
            velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
            force = (offset * m_CrawlerSettings.SpringForce) - (velocity * m_CrawlerSettings.DampForce);
            springForce = direction * force;
        }

        Vector2 moveForce = m_CrawlerSettings.MoveSpeed * m_MoveInput.normalized;
        Vector2 origin = transform.position;

        RaycastHit2D up = Physics2D.Raycast(origin, transform.up, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D down = Physics2D.Raycast(origin, -transform.up, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D right = Physics2D.Raycast(origin, transform.right, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D left = Physics2D.Raycast(origin, -transform.right, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);

        Vector2 verticalForce = Vector2.zero;
        Vector2 horizontalForce = Vector2.zero;

        if (!(up && down))
        {
            if (up)
            {
                direction = (origin - up.point).normalized;
                offset = m_CrawlerSettings.WallSuspensionDistance - up.distance;
                velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
                force = (offset * m_CrawlerSettings.WallSuspensionSpringStrength) - (velocity * m_CrawlerSettings.WallSuspensionDamperStrength);

                verticalForce = direction * force;
            }
            else if (down)
            {
                direction = (origin - down.point).normalized;
                offset = m_CrawlerSettings.WallSuspensionDistance - down.distance;
                velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
                force = (offset * m_CrawlerSettings.WallSuspensionSpringStrength) - (velocity * m_CrawlerSettings.WallSuspensionDamperStrength);

                verticalForce = direction * force;
            }
        }

        if (!(left && right))
        {
            if (left)
            {
                direction = (origin - left.point).normalized;
                offset = m_CrawlerSettings.WallSuspensionDistance - left.distance;
                velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
                force = (offset * m_CrawlerSettings.WallSuspensionSpringStrength) - (velocity * m_CrawlerSettings.WallSuspensionDamperStrength);

                horizontalForce = direction * force;
            }
            else if (right)
            {
                direction = (origin - right.point).normalized;
                offset = m_CrawlerSettings.WallSuspensionDistance - right.distance;
                velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
                force = (offset * m_CrawlerSettings.WallSuspensionSpringStrength) - (velocity * m_CrawlerSettings.WallSuspensionDamperStrength);

                horizontalForce = direction * force;
            }
        }



        m_RigidBody.AddForce(springForce + moveForce + verticalForce + horizontalForce);
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

        if (!GetHits(m_Legs[nextIndex].Orientation) ||
            Vector3.Distance(m_Legs[nextIndex].transform.position, m_BestHit.point) < m_CrawlerSettings.MinimumLegMoveDistance)
        {
            return;
        }

        m_LegIndex = nextIndex;  
        m_Legs[m_LegIndex].StartMove(m_BestHit.transform, m_BestHit.point);
        return;
    }

    private void SetRestPosition()
    {
        Vector2 restPosition = transform.position;

        foreach (Leg leg in m_Legs)
        {
            restPosition += leg.TargetPosition;
        }

        restPosition /= 5f;
        m_RestPosition.position = restPosition;       
    }

    private bool GetHits(Vector2 orientation)
    {
        bool hitFound = false;
        float farthestHit = -1f;
        Vector3 origin = transform.position;
        Vector3 forward = origin + (m_CrawlerSettings.LegReach * orientation.x * transform.right);
        Vector3 up = origin + (m_CrawlerSettings.LegReach * orientation.y * transform.up);
        Vector3 max = origin + (m_CrawlerSettings.LegReach * orientation.x * transform.right) + (m_CrawlerSettings.LegReach * orientation.y * transform.up);

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
                m_BestHit = hit;
                hitFound = true;
            }
        }

        if (!hitFound)
        {
            float closest = 100f;
            Vector3 halfY = origin + (m_CrawlerSettings.LegReach / 2f * orientation.y * transform.up);
            Vector3 halfX = origin + (m_CrawlerSettings.LegReach / 2f * orientation.x * transform.right);
            Vector3 reverseY = origin + (m_CrawlerSettings.LegReach * -orientation.y * transform.up);
            Vector3 reverseX = origin + (m_CrawlerSettings.LegReach * -orientation.x * transform.right);
            Vector3 reverseXMax = origin + (m_CrawlerSettings.LegReach * -orientation.x * transform.right) + (m_CrawlerSettings.LegReach * orientation.y * transform.up);
            Vector3 reverseYMax = origin + (m_CrawlerSettings.LegReach * orientation.x * transform.right) + (m_CrawlerSettings.LegReach * -orientation.y * transform.up);

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
                    m_BestHit = hit;
                    hitFound = true;
                }
            }
        }

        m_TempHits.Clear();

        return hitFound;
    }
}
