using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using System.Threading.Tasks;
using static UnityEngine.UI.Image;
using System.Runtime.InteropServices.WindowsRuntime;

public class Player : MonoBehaviour
{
    private List<RaycastHit2D> m_TempHits = new();
    private RaycastHit2D m_BestHit;
    private Vector2 m_MoveInput = Vector2.zero;
    private Vector2 m_LookInput = Vector2.zero;
    private int m_LegIndex;

    [SerializeField] private CrawlerSettings m_CrawlerSettings;
    [SerializeField] private Rigidbody2D m_RigidBody;
    [SerializeField] private Transform m_RestPosition;
    [SerializeField] private List<Leg> m_Legs = new();  
    [SerializeField] private LayerMask m_LegLayerMask;
    [SerializeField] private Transform m_PlayerGraphic;
    public CrawlerSettings CrawlerSettings { get { return m_CrawlerSettings; } }

    private void Start()
    {
        Physics2D.queriesStartInColliders = false;
        foreach(Leg leg in m_Legs)
        {
            leg.Initialize(m_CrawlerSettings.LegMoveRate);
            GetLegHits(leg.Orientation);
            leg.SnapPosition(m_BestHit.point);
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        m_MoveInput = context.ReadValue<Vector2>();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        m_LookInput = context.ReadValue<Vector2>();
        //LookAtCursor();
    }

    private void Update()
    {     
        MoveLegs();
        SetRestPosition();
        LookAtCursor();
    }

    private void FixedUpdate()
    {
        m_RigidBody.AddForce((m_CrawlerSettings.MoveSpeed * m_MoveInput.normalized) + RestrainToRestPosition() + FloatOffTerrain());
    }

    private void LookAtCursor()
    {
        var lookTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

        m_PlayerGraphic.up = lookTarget - transform.position;
    }

    private Vector2 RestrainToRestPosition()
    {
        Vector2 direction = m_RestPosition.position - transform.position;
        float distance = direction.magnitude;

        if (m_MoveInput != Vector2.zero)
        {
            if (distance < m_CrawlerSettings.MaxSpringStretch)
            {
                
                return Vector2.zero;
            }
        }

        float velocity = Vector2.Dot(direction.normalized, m_RigidBody.linearVelocity);
        float spring = distance > m_CrawlerSettings.MaxSpringStretch ? m_CrawlerSettings.MoveSpeed / distance : m_CrawlerSettings.SpringForce;
        return direction.normalized * ((distance * spring) - (velocity * m_CrawlerSettings.DampForce));
    }

    private Vector2 FloatOffTerrain()
    {
        if(m_MoveInput != Vector2.zero)
            return Vector2.zero;

        m_TempHits.Clear();
        Vector2 finalForce = Vector2.zero;
        RaycastHit2D up = Physics2D.Raycast(transform.position, transform.up, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D down = Physics2D.Raycast(transform.position, -transform.up, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D right = Physics2D.Raycast(transform.position, transform.right, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D left = Physics2D.Raycast(  transform.position, -transform.right, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);   
        
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

        return finalForce;
    }

    private void MoveLegs()
    {
        if (m_MoveInput == Vector2.zero)
        {
            bool shouldMove = false;

            foreach (var leg in m_Legs)
            {
                if (Vector2.Distance(transform.position, leg.transform.position) > m_CrawlerSettings.MaxLegDistance)
                {
                    shouldMove = true;
                    break;
                }
            }

            if (!shouldMove)
            {
                return;
            }
        }

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
        m_LegIndex = nextIndex;

        if (!GetLegHits(m_Legs[nextIndex].Orientation) ||
            Vector3.Distance(m_Legs[nextIndex].TargetPosition, m_BestHit.point) < m_CrawlerSettings.MinimumLegMoveDistance)
        {
            return;
        }

        
        m_Legs[m_LegIndex].SetTarget(m_BestHit.transform, m_BestHit.point, m_BestHit.normal);
        m_Legs[m_LegIndex].StartMove();
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
             
        float maxDistance = m_CrawlerSettings.LegReach;
        Vector3 origin = transform.position;
        Vector3 forward = origin + (m_CrawlerSettings.LegReach * orientation.x * transform.right);
        Vector3 up = origin + (m_CrawlerSettings.LegReach * orientation.y * transform.up);
        Vector3 max = origin + (m_CrawlerSettings.LegReach * orientation.x * transform.right) + (m_CrawlerSettings.LegReach * orientation.y * transform.up);
        Vector3 halfY = origin + (m_CrawlerSettings.LegReach / 2f * orientation.y * transform.up);
        Vector3 halfX = origin + (m_CrawlerSettings.LegReach / 2f * orientation.x * transform.right);
        Vector3 reverseY = origin + (m_CrawlerSettings.LegReach * -orientation.y * transform.up);
        Vector3 reverseX = origin + (m_CrawlerSettings.LegReach * -orientation.x * transform.right);
        RaycastHit2D originToMax = Physics2D.Raycast(origin, max - origin, Vector3.Distance(origin, max), m_LegLayerMask);
        RaycastHit2D forwardToMax = Physics2D.Raycast(forward, max - forward, Vector3.Distance(forward, max), m_LegLayerMask);
        RaycastHit2D upToMax = Physics2D.Raycast(up, max - up, Vector3.Distance(up, max), m_LegLayerMask);
        RaycastHit2D originToForward = Physics2D.Raycast(origin, forward - origin, Vector3.Distance(origin, forward), m_LegLayerMask);
        RaycastHit2D originToUp = Physics2D.Raycast(origin, up - origin, Vector3.Distance(up, origin), m_LegLayerMask);
        m_TempHits.Clear();

        if (!forwardToMax)
        {
            m_TempHits.Add(Physics2D.Raycast(max, forward - max, Vector3.Distance(max, forward), m_LegLayerMask));
        }

        if (!upToMax)
        {
            m_TempHits.Add(Physics2D.Raycast(max, up - max, Vector3.Distance(max, up), m_LegLayerMask));
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

        if(FindFarthestHit()) 
            return true;

        m_TempHits.Clear();
        if (!originToMax)
        {
            m_TempHits.Add(Physics2D.Raycast(max, origin - max, Vector3.Distance(max, origin), m_LegLayerMask));
        }

        if(FindFarthestHit()) 
            return true;

        m_TempHits.Clear();
        m_TempHits.Add(originToMax);

        if(FindFarthestHit())
            return true;

        m_TempHits.Clear();
        m_TempHits.Add(Physics2D.Raycast(halfX, reverseY - halfX, Vector3.Distance(halfX, reverseY), m_LegLayerMask));
        m_TempHits.Add(Physics2D.Raycast(halfY, reverseX - halfY, Vector3.Distance(halfY, reverseX), m_LegLayerMask)); 

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
}
