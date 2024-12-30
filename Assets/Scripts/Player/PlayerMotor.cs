using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class PlayerMotor : MonoBehaviour
{
    private List<Leg> m_LegsWithoutPosition = new();
    private List<Leg> m_TempLegs = new();
    private List<RaycastHit2D> m_TempHits = new();
    private ContactPoint2D[] m_Contacts;
    private RaycastHit2D m_BestHit;
    private Vector3 m_LastRestPosition;
    
    private int m_LegIndex;   
    
    private bool m_ShouldStopFlying;
    
    [SerializeField] private Player m_Player;
    [SerializeField] private List<Leg> m_Legs = new();    
    [SerializeField] private CrawlerSettings m_CrawlerSettings;
    [SerializeField] private Rigidbody2D m_RigidBody;     
    [SerializeField] private Transform m_RestPosition;
    [SerializeField] private LayerMask m_LegLayerMask;
    [SerializeField] private int m_MaxContacts;

    private void Awake()
    {
        m_Contacts = new ContactPoint2D[m_MaxContacts];
    }

    public void Initialize()
    {
        foreach (Leg leg in m_Legs)
        {
            leg.Initialize(m_CrawlerSettings.LegMoveRate);
            GetLegHits(leg.Orientation);
            leg.SnapPosition(m_BestHit.point);
        }
    }

    public void EndLaunch()
    {
        m_RigidBody.gravityScale = 0f;
        m_RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        m_RigidBody.linearDamping = 10f;
    }

    public void LaunchPlayer()
    {
        m_LegsWithoutPosition.Clear();
        foreach (var leg in m_Legs)
        {
            leg.PlayerLaunched();
            m_LegsWithoutPosition.Add(leg);
        }

        m_RigidBody.gravityScale = 1f;
        m_RigidBody.constraints = RigidbodyConstraints2D.None;
        m_RigidBody.linearDamping = 1f;
    }

    public Vector2 GetLaunchForce()
    {
        Vector3 direction = m_RestPosition.position - transform.position;
         return direction * m_CrawlerSettings.LaunchForce;
    }

    public void MovePlayer(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
    {
        m_RigidBody.AddForce(force, forceMode);
    }

    public bool IsMouseInDeadZone()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        return Vector2.Distance(mouseWorldPosition, (Vector2)transform.position) < m_CrawlerSettings.CrosshairLookThreshold;
    }

    public void PlayerInAir()
    {
        m_ShouldStopFlying = false;
    }

    public bool CanPlayerLand(float timeStartedFalling)
    {
        bool canLand = false;

        if(m_ShouldStopFlying || m_RigidBody.linearVelocity.magnitude < m_CrawlerSettings.LaunchHitVelocityThreshold)
        {
            canLand = true;
        }

        return canLand;
    }

    public bool IsPlayersVelocityBelowLandingThreshold()
    {
        return m_RigidBody.linearVelocity.magnitude < m_CrawlerSettings.LaunchHitVelocityThreshold;
    }

    public Vector2 GetMoveForce(Vector2 moveInput)
    {
        return m_CrawlerSettings.MoveSpeed * moveInput.normalized;
    }

    public Vector2 GetStretchMoveForce()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        Vector2 direction = (mouseWorldPosition - (Vector2)transform.position).normalized;
        return direction * m_CrawlerSettings.MoveSpeed;
    }

    public Vector2 RestrainToRestPosition()
    {
        float restPositionVelocity = (m_RestPosition.position - m_LastRestPosition).magnitude;
        Vector2 direction = m_RestPosition.position - transform.position;
        float distance = direction.magnitude;
        float velocity = Vector2.Dot(direction.normalized, m_RigidBody.linearVelocity);
        float spring = distance > m_CrawlerSettings.MaxSpringStretch ? (m_CrawlerSettings.MoveSpeed / distance) + (Mathf.Clamp(distance - m_CrawlerSettings.MaxSpringStretch - .3f, 0, 10f) * (m_CrawlerSettings.MoveSpeed / distance))  : m_CrawlerSettings.SpringForce;
        return direction.normalized * ((distance * spring) - (velocity * m_CrawlerSettings.DampForce));
    }
    public Vector2 FloatOffTerrain()
    {
        m_TempHits.Clear();
        Vector2 finalForce = Vector2.zero;
        RaycastHit2D up = Physics2D.Raycast(transform.position, transform.up, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D down = Physics2D.Raycast(transform.position, -transform.up, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D right = Physics2D.Raycast(transform.position, transform.right, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D left = Physics2D.Raycast(transform.position, -transform.right, m_CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);

        if (up != down)
        {
            m_TempHits.Add(up ? up : down);
        }

        if (left != right)
        {
            m_TempHits.Add(left ? left : right);
        }

        foreach (var hit in m_TempHits)
        {
            Vector2 direction = (m_RigidBody.position - hit.point).normalized;
            float offset = m_CrawlerSettings.WallSuspensionDistance - hit.distance;
            float velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
            float force = (offset * m_CrawlerSettings.WallSuspensionSpringStrength) - (velocity * m_CrawlerSettings.WallSuspensionDamperStrength);
            finalForce += direction * force;
        }

        return finalForce;
    }
    public void SetRestPosition()
    {
        m_LastRestPosition = m_RestPosition.position;
        Vector2 restPosition = transform.position;
        restPosition = Vector2.zero;
        int counter = 0;

        foreach (Leg leg in m_Legs)
        {
            if (!m_LegsWithoutPosition.Contains(leg))
            {
                counter++;
                restPosition += (Vector2)leg.transform.position;
            }
        }

        //restPosition /= counter + 1;
        if (counter > 0)
        {
            restPosition /= counter;
        }
        else
        {
            restPosition = transform.position;
        }


        m_RestPosition.position = restPosition;
    }

    public bool IsPlayerBeyondLaunchThreshold()
    {
        return Vector3.Distance(m_RestPosition.position, transform.position) > m_CrawlerSettings.LaunchThreshold;
    }

    public void CheckLegsWithoutPosition()
    {
        m_TempLegs.Clear();
        foreach (var leg in m_LegsWithoutPosition)
        {
            if (GetLegHits(leg.Orientation))
            {
                leg.SetTarget(m_BestHit.transform, m_BestHit.point, m_BestHit.normal);
                leg.FreezeConstraints();
                m_TempLegs.Add(leg);
            }
        }

        foreach(var leg in m_TempLegs)
        {
            m_LegsWithoutPosition.Remove(leg);
        }
    }

    public bool LegNeedsToMove()
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

        return shouldMove;
    }

    public bool EnoughLegsToWalk()
    {
        return m_LegsWithoutPosition.Count < 3;
    }

    public bool HasPlayerHitMoveThreshold()
    {
        return Vector3.Distance(transform.position, m_RestPosition.position) >= m_CrawlerSettings.LegMoveThreshold;
    }

    public void CheckCurrentLeg()
    {
        if (!m_Legs[m_LegIndex].IsDoneMoving())
        {    
            return;
        }

        int nextIndex = m_LegIndex + 1;

        if (nextIndex == m_Legs.Count)
        {
            nextIndex = 0;
        }
        m_LegIndex = nextIndex;


        if (!GetLegHits(m_Legs[nextIndex].Orientation) || Vector3.Distance(m_Legs[nextIndex].TargetPosition, m_BestHit.point) < m_CrawlerSettings.MinimumLegMoveDistance)
        {
            return;
        }


        


        m_Legs[m_LegIndex].SetTarget(m_BestHit.transform, m_BestHit.point, m_BestHit.normal);
        return;
    }

    public bool IsPlayerAtMaxDistance()
    {
        return Vector3.Distance(transform.position, m_RestPosition.position) >= m_CrawlerSettings.MaxSpringStretch;
    }

    private bool GetLegHits(Vector2 orientation)
    {
        Vector3 origin = transform.position;
        Vector3 offsetOrigin = transform.position + ((transform.right * m_Player.PlayerInput.MoveInput.x) + (transform.up * m_Player.PlayerInput.MoveInput.y)) * m_CrawlerSettings.LegSearchMultiplier;

        if (!Physics2D.Raycast(transform.position, offsetOrigin - transform.position, Vector3.Distance(offsetOrigin, transform.position), m_LegLayerMask))
        {
            origin = offsetOrigin; 
        }
        
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

        if (FindFarthestHit())
            return true;

        m_TempHits.Clear();
        if (!originToMax)
        {
            m_TempHits.Add(Physics2D.Raycast(max, origin - max, Vector3.Distance(max, origin), m_LegLayerMask));
        }

        if (FindFarthestHit())
            return true;

        m_TempHits.Clear();
        m_TempHits.Add(originToMax);

        if (FindFarthestHit())
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
            if (!hit || hit.collider.gameObject.layer != 6)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.GetContacts(m_Contacts);

        foreach (var contact in m_Contacts)
        {
            if(Vector2.Angle(m_RigidBody.linearVelocity, contact.normal) > m_CrawlerSettings.MaxCollisionAngle)
            {
                m_ShouldStopFlying = true;
            }
        }
    }
}
