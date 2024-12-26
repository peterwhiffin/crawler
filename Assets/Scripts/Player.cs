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
    private Vector2 m_MoveInput = Vector2.zero;
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
            GetLegHits(leg.Orientation);
            leg.StartMove(m_BestHit.transform, m_BestHit.point);
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        m_MoveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {     
        MoveLegs();
        SetRestPosition();
    }

    private void FixedUpdate()
    {
        m_RigidBody.AddForce((m_CrawlerSettings.MoveSpeed * m_MoveInput.normalized) + RestrainToRestPosition() + FloatOffTerrain());
    }

    private Vector2 RestrainToRestPosition()
    {
        Vector2 direction = m_RestPosition.position - transform.position;
        float distance = direction.magnitude;
        float velocity = Vector2.Dot(direction.normalized, m_RigidBody.linearVelocity);
        float spring = distance > m_CrawlerSettings.MaxSpringStretch ? m_CrawlerSettings.MoveSpeed / distance : m_CrawlerSettings.SpringForce;
        return direction.normalized * ((distance * spring) - (velocity * m_CrawlerSettings.DampForce));
    }

    private Vector2 FloatOffTerrain()
    {
        RaycastHit2D up = Physics2D.Raycast(transform.position, transform.up, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D down = Physics2D.Raycast(transform.position, -transform.up, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D right = Physics2D.Raycast(transform.position, transform.right, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D left = Physics2D.Raycast(transform.position, -transform.right, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);        
        Vector2 finalForce = Vector2.zero;

        if (up != down)
        {
            m_TempHits.Add(up ? up : down);
        }

        if (left != right)
        {
            m_TempHits.Add(left ? left : right);
        }

        foreach(var hit in m_TempHits)
        {
            Vector2 direction = (m_RigidBody.position - hit.point).normalized;
            float offset = m_CrawlerSettings.WallSuspensionDistance - hit.distance;
            float velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
            float force = (offset * m_CrawlerSettings.WallSuspensionSpringStrength) - (velocity * m_CrawlerSettings.WallSuspensionDamperStrength);
            finalForce += direction * force;
        }

        m_TempHits.Clear();
        return finalForce;
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

        if (!GetLegHits(m_Legs[nextIndex].Orientation) ||
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

    private bool GetLegHits(Vector2 orientation)
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
