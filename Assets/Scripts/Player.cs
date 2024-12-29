using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private List<RaycastHit2D> m_TempHits = new();
    private List<Leg> m_AvailableLegs = new();
    private RaycastHit2D m_BestHit;
    private Vector2 m_MoveInput = Vector2.zero;
    private Vector2 m_LookInput = Vector2.zero;
    private bool m_FreezeLegsInput = false;
    private int m_LegIndex;
    private bool m_QueueLaunch = false;
    private bool m_IsLaunching = false;
    private float m_LaunchTime = 0f;
    private bool m_HitWhileLaunching = false;

    [SerializeField] private Transform m_Crosshair;
    [SerializeField] private CrawlerSettings m_CrawlerSettings;
    [SerializeField] private Rigidbody2D m_RigidBody;
    [SerializeField] private Transform m_RestPosition;
    [SerializeField] private List<Leg> m_Legs = new();  
    [SerializeField] private LayerMask m_LegLayerMask;
    [SerializeField] private Transform m_PlayerGraphic;
    [SerializeField] private HotBar m_Hotbar;
    public CrawlerSettings CrawlerSettings { get { return m_CrawlerSettings; } }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
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
    }

    public void OnFreezeLegsInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_FreezeLegsInput = true;
            
        }
        
        if (context.canceled)
        {
            m_FreezeLegsInput = false;

            if (!m_IsLaunching)
            {
                if (Vector3.Distance(m_RestPosition.position, transform.position) > m_CrawlerSettings.LaunchThreshold)
                {
                    m_QueueLaunch = true;
                }
            }
        }
    }

    private void Update()
    {
        if (m_IsLaunching)
        {
            if(Time.time - m_LaunchTime > m_CrawlerSettings.LaunchTime || m_HitWhileLaunching)
            {
                int counter = 0;

                foreach (var leg in m_Legs)
                {

                    if (GetLegHits(leg.Orientation))
                    {
                        counter++;
                        leg.PlayerLanded();
                        leg.NewTarget(m_BestHit.transform, m_BestHit.point, m_BestHit.normal);
                    }
                }

                if (counter >= 1)
                {

                    m_HitWhileLaunching = false;
                    m_IsLaunching = false;
                    m_RigidBody.gravityScale = 0f;
                    m_RigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    m_RigidBody.linearDamping = 10f;
                }
            }
            else
            {
                return;
            }
        }


        MoveLegs();
        SetRestPosition();
        LookAtCursor();
    }

    private void FixedUpdate()
    {
        if (m_QueueLaunch)
        {
            foreach(var leg in m_Legs)
            {
                leg.PlayerLaunched();
            }

            m_QueueLaunch = false;
            m_IsLaunching = true;
            m_RigidBody.gravityScale = 1f;
            m_RigidBody.constraints = RigidbodyConstraints2D.None;
            m_RigidBody.linearDamping = 1f;
            m_LaunchTime = Time.time;
            Vector3 direction = m_RestPosition.position - transform.position;
            Vector3 force = direction * m_CrawlerSettings.LaunchForce;
            m_RigidBody.AddForce(force, ForceMode2D.Impulse);
        }
        else
        {
            if (m_IsLaunching)
                return;

            m_RigidBody.AddForce(GetMoveForce() + RestrainToRestPosition() + FloatOffTerrain());
        }
    }

    private Vector2 GetMoveForce()
    {
        Vector2 force = Vector2.zero;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        if (m_FreezeLegsInput && m_MoveInput == Vector2.zero && Vector2.Distance(mouseWorldPosition, (Vector2)transform.position) > m_CrawlerSettings.CrosshairLookThreshold)
        {
            Vector2 direction = (mouseWorldPosition - (Vector2)transform.position).normalized;
            force = direction * m_CrawlerSettings.MoveSpeed;
        }
        else
        {
            force = m_CrawlerSettings.MoveSpeed * m_MoveInput.normalized;
        }

        return force;
    }


    private void LookAtCursor()
    {
        Vector2 lookTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));


        if (m_FreezeLegsInput && m_MoveInput == Vector2.zero && !m_Hotbar.IsAttacking)
        {
            Vector2 direction = ((Vector2)m_RestPosition.position - (Vector2)transform.position).normalized;
            Vector2 lookTarget2 = direction * 2f;

            m_PlayerGraphic.up = lookTarget2;
        }
        else
        {

            if(Vector2.Distance(lookTarget, (Vector2)transform.position) > m_CrawlerSettings.CrosshairLookThreshold)
                m_PlayerGraphic.up = lookTarget - (Vector2)transform.position;            
        }

        m_Crosshair.position = lookTarget;
    }

    private Vector2 RestrainToRestPosition()
    {
        Vector2 direction = m_RestPosition.position - transform.position;
        float distance = direction.magnitude;

        if (m_MoveInput != Vector2.zero || m_FreezeLegsInput)
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
        if(m_MoveInput != Vector2.zero || m_FreezeLegsInput)
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
        foreach(var leg in m_Legs)
        {
            if (leg.NeedsNewPosition)
            {
                if (GetLegHits(leg.Orientation))
                {
                    leg.PlayerLanded();
                    leg.NewTarget(m_BestHit.transform, m_BestHit.point, m_BestHit.normal);
                }
            }
        }

        if (m_FreezeLegsInput)
            return;

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

        
        m_Legs[m_LegIndex].NewTarget(m_BestHit.transform, m_BestHit.point, m_BestHit.normal);
        return;
    }

    private void SetRestPosition()
    {
        Vector2 restPosition = transform.position;
        int counter = 0;

        foreach (Leg leg in m_Legs)
        {
            if (!leg.NeedsNewPosition)
            {
                counter++;
                restPosition += leg.TargetPosition;
            }
        }

        restPosition /= counter + 1;
        m_RestPosition.position = restPosition;       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_IsLaunching)
        {
            if (m_RigidBody.linearVelocity.magnitude < m_CrawlerSettings.LaunchHitVelocityThreshold)
            {
                m_HitWhileLaunching = true;
            }
        }
    }
}
