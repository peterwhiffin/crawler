using System;
using System.Collections.Generic;
using UnityEngine;


public class Leg : MonoBehaviour
{
    private float m_TimeElapsed;    
    private bool m_IsFalling = false;
    private bool m_NeedNewPosition = false;
    private float m_LastFootStepTime;
    private Vector3[] m_RopePositions;

    [SerializeField] private Player m_Player;
    [SerializeField] private Transform m_TargetPosition;
    [SerializeField] private Vector2 m_Orientation;
    //[SerializeField] private CrawlerSettings m_CrawlerSettings;
    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] private Rigidbody2D m_RigidBody;
    [SerializeField] private int m_RopeResolution;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private Transform m_RopeTarget;
    

    private bool m_FootStepPlayed = false;

    private List<Vector3> allRopeSections = new();
    public Vector2 Orientation { get { return m_Orientation; } }
    public Vector2 TargetPosition { get { return m_TargetPosition.position; } }
    public bool NeedsNewPosition { get { return m_NeedNewPosition; } }

    private void Awake()
    {
        m_RopePositions = new Vector3[m_RopeResolution];
        m_LastFootStepTime = Time.time;
    }

    public void Initialize(float moveRate)
    {
        m_TimeElapsed = 0f;
    }

    private void Update()
    {
        if (m_IsFalling)
        {
            return;
        }
        float legMoveRate = m_Player.CrawlerSettings.LegMoveRateCurve.Evaluate(m_Player.CrawlerSettings.MoveSpeed);
        if (m_TimeElapsed > legMoveRate)
        {
            if (!m_FootStepPlayed && Time.time - m_LastFootStepTime > m_Player.CrawlerSettings.FootStepRate)
            {
                int randomIndex = UnityEngine.Random.Range(0, m_Player.CrawlerSettings.FootStepClips.Count);
                m_AudioSource.PlayOneShot(m_Player.CrawlerSettings.FootStepClips[randomIndex]);
                m_FootStepPlayed = true;
                m_LastFootStepTime = Time.time;
            }

            transform.position = m_TargetPosition.position;
            return;
        }

        m_FootStepPlayed = false;
        

        //float time = m_TimeElapsed / m_CrawlerSettings.LegMoveRate;
        float time = m_TimeElapsed / legMoveRate;
        Vector3 target = m_TargetPosition.position + m_TargetPosition.up * m_Player.CrawlerSettings.StepHeight * m_Player.CrawlerSettings.StepCurve.Evaluate(time);
        transform.position = Vector3.Lerp(transform.position, target, time);

        m_TimeElapsed += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (m_IsFalling)
        {
            m_RigidBody.AddForce(GetSpringForce());
        }
    }

    public void FreezeConstraints()
    {
        m_RigidBody.bodyType = RigidbodyType2D.Kinematic;
        m_RigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private Vector2 GetSpringForce()
    {
        Vector2 direction = m_RigidBody.position - (Vector2)m_Player.transform.position;
        float offset = m_Player.CrawlerSettings.LegLaunchRestDistance - direction.magnitude;
        float velocity = Vector2.Dot(direction.normalized, m_RigidBody.GetPointVelocity(m_RigidBody.position));
        float force = (offset * m_Player.CrawlerSettings.LegLaunchSpringStrength) + (velocity * m_Player.CrawlerSettings.LegLaunchDamperStrength);
        return direction.normalized * force;
    }

    public void PlayerLaunched()
    {
        m_IsFalling = true;
        m_RigidBody.bodyType = RigidbodyType2D.Dynamic;
        m_RigidBody.constraints = RigidbodyConstraints2D.None;
        m_RigidBody.linearDamping = m_Player.CrawlerSettings.LegLinearDamping;
        m_NeedNewPosition = true;
        m_TimeElapsed = 0f;
    }

    public void SetTarget(Transform parent, Vector2 targetPosition, Vector2 targetNormal)
    {
        m_TargetPosition.SetParent(null);
        m_TargetPosition.up = targetNormal;
        m_TargetPosition.SetParent(parent);
        m_TargetPosition.position = targetPosition;     
        transform.up = targetNormal;
        m_TimeElapsed = 0f;
        m_IsFalling = false;
    }

    public bool IsDoneMoving()
    {
        float legMoveRate = m_Player.CrawlerSettings.LegMoveRateCurve.Evaluate(m_Player.CrawlerSettings.MoveSpeed);
        return m_TimeElapsed > legMoveRate;
    }

    public void SnapPosition(Vector3 position)
    {
        transform.position = position;
    }
}
