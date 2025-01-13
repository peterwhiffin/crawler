using UnityEngine;
using System.Collections.Generic;


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
    //[SerializeField] private CrawlerSettings m_CrawlerSettings;
    [SerializeField] private Rigidbody2D m_RigidBody;     
    [SerializeField] private Transform m_RestPosition;
    [SerializeField] private LayerMask m_LegLayerMask;
    [SerializeField] private int m_MaxContacts;

    public bool IsGrappled = false;
    public Vector2 GrapplePosition;
    private float m_LaunchTimer;
    private float m_GrappleForceMod;
    private int m_LegSearchCounter = 0;
    private bool m_StepTargetFound = false;

    public Rigidbody2D Rigidbody {  get { return m_RigidBody; } }
    private void Awake()
    {
        m_Contacts = new ContactPoint2D[m_MaxContacts];
    }

    public void Initialize()
    {
        foreach (Leg leg in m_Legs)
        {
            leg.Initialize(m_Player.CrawlerSettings.LegMoveRate);
            GetLegHits(leg.Orientation);
            leg.SnapPosition(m_BestHit.point);
           leg.SetTarget(m_BestHit.transform, m_BestHit.point, m_BestHit.normal);
        }
    }

    public Vector2 GetGrappleForce()
    {
        IsGrappled = false;
        return m_Player.CrawlerSettings.GrappleForce * m_GrappleForceMod * (GrapplePosition - (Vector2)transform.position).normalized;
    }

    public void HookGrapple(Vector2 position, float forceMod)
    {
        IsGrappled = true;
        GrapplePosition = position;
        m_GrappleForceMod = forceMod;
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
         return direction * m_Player.CrawlerSettings.LaunchForce;
    }

    public void MovePlayer(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
    {
        m_RigidBody.AddForce(force, forceMode);
    }

    public bool IsMouseInDeadZone()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        return Vector2.Distance(mouseWorldPosition, (Vector2)transform.position) < m_Player.CrawlerSettings.CrosshairLookThreshold;
    }

    public void PlayerInAir()
    {
        m_ShouldStopFlying = false;
    }

    public void SetLaunchTimer()
    {
        m_LaunchTimer = m_Player.CrawlerSettings.LaunchTimeout;
    }

    public bool CanPlayerLand(float timeStartedFalling)
    {
        if(Time.time - timeStartedFalling > m_LaunchTimer)
        {
            m_LaunchTimer = 0f;
            return true;
        }

        bool canLand = false;

        if(m_ShouldStopFlying || m_RigidBody.linearVelocity.magnitude < m_Player.CrawlerSettings.LaunchHitVelocityThreshold)
        {
            canLand = true;
        }

        return canLand;
    }

    public bool IsPlayersVelocityBelowLandingThreshold()
    {
        return m_RigidBody.linearVelocity.magnitude < m_Player.CrawlerSettings.LaunchHitVelocityThreshold;
    }

    public Vector2 GetMoveForce(Vector2 moveInput)
    {
        return m_Player.CrawlerSettings.MoveSpeed * moveInput.normalized;
    }

    public Vector2 GetStretchMoveForce()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        Vector2 direction = (mouseWorldPosition - (Vector2)transform.position).normalized;
        return direction * m_Player.CrawlerSettings.MoveSpeed;
    }

    public Vector2 RestrainToRestPosition()
    {
        float restPositionVelocity = (m_RestPosition.position - m_LastRestPosition).magnitude;
        Vector2 direction = m_RestPosition.position - transform.position;
        float distance = direction.magnitude;
        float velocity = Vector2.Dot(direction.normalized, m_RigidBody.linearVelocity);
        float spring = distance > m_Player.CrawlerSettings.MaxSpringStretch ? (m_Player.CrawlerSettings.MoveSpeed / distance) + (Mathf.Clamp(distance - m_Player.CrawlerSettings.MaxSpringStretch - .3f, 0, 10f) * (m_Player.CrawlerSettings.MoveSpeed / distance))  : m_Player.CrawlerSettings.SpringForce;
        return direction.normalized * ((distance * spring) - (velocity * m_Player.CrawlerSettings.DampForce));
    }
    public Vector2 FloatOffTerrain()
    {
        m_TempHits.Clear();
        Vector2 finalForce = Vector2.zero;
        RaycastHit2D up = Physics2D.Raycast(transform.position, transform.up, m_Player.CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D down = Physics2D.Raycast(transform.position, -transform.up, m_Player.CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D right = Physics2D.Raycast(transform.position, transform.right, m_Player.CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);
        RaycastHit2D left = Physics2D.Raycast(transform.position, -transform.right, m_Player.CrawlerSettings.WallSuspensionDistance, m_LegLayerMask);

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
            Vector2 direction = ((Vector2)transform.position - hit.point);
            float offset = m_Player.CrawlerSettings.WallSuspensionDistance - hit.distance;
            float velocity = Vector2.Dot(direction, m_RigidBody.linearVelocity);
            float force = (offset * m_Player.CrawlerSettings.WallSuspensionSpringStrength) - (velocity * m_Player.CrawlerSettings.WallSuspensionDamperStrength);
            finalForce += direction.normalized * force;
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
        return Vector3.Distance(m_RestPosition.position, transform.position) > m_Player.CrawlerSettings.LaunchThreshold;
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
            if (Vector2.Distance(transform.position, leg.transform.position) > m_Player.CrawlerSettings.MaxLegDistance)
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
        return Vector3.Distance(transform.position, m_RestPosition.position) >= m_Player.CrawlerSettings.LegMoveThreshold;
    }

    public Vector2 GetJumpDirection()
    {
        Vector2 direction = Vector2.zero;

        foreach(var leg in m_Legs)
        {
            direction += (Vector2)leg.transform.position;
        }

        direction /= 4;

        direction = direction - (Vector2)transform.position;

        return -direction.normalized;
    }

    public void CheckLegDistance(Leg leg)
    {
        if (Vector3.Distance(leg.TargetPosition, transform.position) > m_Player.CrawlerSettings.MaxLegDistance)
        {
            BreakLeg(leg);
        }
    }

    public void CheckLegsInAir()
    {
        foreach(var leg in m_Legs)
        {
            CheckLegDistance(leg);
        }
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

        //bool foundHit = false;

        //for(int i = 0; i < m_Legs.Count; i++)
        //{
        //    if (GetLegHits(m_Legs[nextIndex].Orientation))
        //    {
        //        foundHit = true;
        //        break;
        //    }
        //    else if(Vector3.Distance(m_Legs[nextIndex].TargetPosition, m_BestHit.point) < m_Player.CrawlerSettings.MinimumLegMoveDistance)
        //    {
        //        CheckLegDistance(m_Legs[nextIndex]);
        //    }

        //    nextIndex++;

        //    if(nextIndex == m_Legs.Count)
        //    {
        //        nextIndex = 0;
        //    }
        //}

        //m_LegIndex = nextIndex;

        //if (!foundHit)
        //{
        //    return;
        //}

        if (!GetLegHits(m_Legs[nextIndex].Orientation))
        {
            CheckLegDistance(m_Legs[nextIndex]);
            return;
        }
        else if (Vector3.Distance(m_Legs[nextIndex].TargetPosition, m_BestHit.point) < m_Player.CrawlerSettings.MinimumLegMoveDistance)
        {
            CheckLegDistance(m_Legs[nextIndex]);
            return;
        }

        m_Legs[m_LegIndex].SetTarget(m_BestHit.transform, m_BestHit.point, m_BestHit.normal);
        return;
    }

    public void BreakLeg(Leg leg)
    {
        leg.PlayerLaunched();

        if (!m_LegsWithoutPosition.Contains(leg))
        {
            m_LegsWithoutPosition.Add(leg);
        }
    }

    public bool IsPlayerAtMaxDistance()
    {
        return Vector3.Distance(transform.position, m_RestPosition.position) >= m_Player.CrawlerSettings.MaxSpringStretch;
    }

    public bool IsPlayerAtMaxStretchDistance()
    {
        return Vector3.Distance(transform.position, m_RestPosition.position) >= m_Player.CrawlerSettings.MaxStretchDistance;
    }

    private bool GetLegHits(Vector2 orientation, bool retry = false)
    {
        Vector3 origin = transform.position;
        Vector3 offset = ((transform.right * m_Player.PlayerInput.MoveInput.x) + (transform.up * m_Player.PlayerInput.MoveInput.y)) * m_Player.CrawlerSettings.LegSearchMultiplier;
        var reach = m_Player.CrawlerSettings.LegReach;
        
        if (!Physics2D.Raycast(transform.position, origin + offset - transform.position, Vector3.Distance(origin + offset, transform.position), m_LegLayerMask))
        {
            origin += offset;
            reach += offset.magnitude;
        }

        if (retry)
        {
            origin = transform.position;
            reach = m_Player.CrawlerSettings.LegReach;
        }

        Vector3 forward = origin + (reach * orientation.x * transform.right);
        Vector3 up = origin + (reach * orientation.y * transform.up);
        Vector3 max = origin + (reach * orientation.x * transform.right) + (reach * orientation.y * transform.up);
        Vector3 halfY = origin + (reach / 2f * orientation.y * transform.up);
        Vector3 halfX = origin + (reach / 2f * orientation.x * transform.right);
        Vector3 reverseY = origin + (reach * -orientation.y * transform.up);
        Vector3 reverseX = origin + (reach * -orientation.x * transform.right);
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
        m_TempHits.Add(originToUp);
        m_TempHits.Add(originToForward);
        if (FindFarthestHit())
            return true;


        if (!retry)
        {
            if(GetLegHits(orientation, true))
            {
                return true;
            }
        }
        //m_TempHits.Clear();

        //m_TempHits.Add(originToUp);
        //m_TempHits.Add(originToForward);

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
            if(Vector2.Angle(m_RigidBody.linearVelocity, contact.normal) > m_Player.CrawlerSettings.MaxCollisionAngle)
            {
                m_ShouldStopFlying = true;
            }
        }
    }
}
