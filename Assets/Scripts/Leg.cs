using System;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public enum Side { Left, Right, Top, Bottom }

    [Serializable]
    public struct LegOrientation
    {
        public int horizontal;
        public int vertical;


    }
    
    public Transform m_targetPosition;
    public Player m_Player;
    public bool m_IsDoneMoving;
    public Transform m_RayPosition;
    public bool m_IsHosted;
    public bool m_NeedsToMove;
    private Vector3 m_SmoothVelocity;
    [SerializeField] public LegOrientation m_Orientation;
    private float m_TimeElapsed;

    private void Start()
    {
        //SearchForTargetPosition(Vector3.zero);
        //transform.position = m_targetPosition;
        m_IsDoneMoving = false;
        m_NeedsToMove = false;
    }

    public void SearchForTargetPosition(Vector3 direction)
    {
        RaycastHit2D closesHit;
        RaycastHit2D hitRight = Physics2D.BoxCast(m_RayPosition.position, new Vector2(transform.localScale.x, transform.localScale.y), 0f, direction, m_Player.m_MaxRayDistance, m_Player.m_LegLayerMask);
        RaycastHit2D hitLeft = Physics2D.BoxCast(m_RayPosition.position, new Vector2(transform.localScale.x, transform.localScale.y), 0f, direction, m_Player.m_MaxRayDistance, m_Player.m_LegLayerMask);
        RaycastHit2D hitUp = Physics2D.BoxCast(m_RayPosition.position, new Vector2(transform.localScale.x, transform.localScale.y), 0f, direction, m_Player.m_MaxRayDistance, m_Player.m_LegLayerMask);
        RaycastHit2D hitDown = Physics2D.BoxCast(m_RayPosition.position, new Vector2(transform.localScale.x, transform.localScale.y), 0f, direction, m_Player.m_MaxRayDistance, m_Player.m_LegLayerMask);

        closesHit = hitRight;

        if (hitLeft)
        {
            if (closesHit)
            {
                if (hitLeft.distance < closesHit.distance)
                {
                    closesHit = hitLeft;
                }
            }
            else
            {
                closesHit = hitLeft;
            }
        }

        if (hitUp)
        {
            if (closesHit)
            {
                if (hitUp.distance < closesHit.distance)
                {
                    closesHit = hitUp;
                }
            }
            else
            {
                closesHit = hitUp;
            }
        }

        if (hitDown)
        {
            if (closesHit)
            {
                if (hitDown.distance < closesHit.distance)
                {
                    closesHit = hitDown;
                }
            }
            else
            {
                closesHit = hitDown;
            }
        }

        if (closesHit)
        {
            Debug.Log("hit: " + closesHit.collider.name);
            m_targetPosition.position = closesHit.centroid;
            m_IsDoneMoving = false;
        }
    }

    private void Update()
    {

       

        //if (transform.position == m_targetPosition)
        //{
        //    m_IsDoneMoving = true;
        //}

    }

    public bool MoveLeg()
    {
        bool isDone = false;


        //if (Vector3.Distance(transform.position, m_targetPosition) < m_Player.m_MinimumLegMoveDistance && m_NeedsToMove)
        //{
        //    return false;
        //}
        if (m_NeedsToMove)
        {
            m_TimeElapsed = 0f;
        }

        transform.position = Vector3.Lerp(transform.position, m_targetPosition.position , m_TimeElapsed / m_Player.m_LegMoveRate);
        //transform.position = Vector3.SmoothDamp(transform.position, m_targetPosition, ref m_SmoothVelocity, m_Player.m_LegMoveRate);
        m_NeedsToMove = false;
        m_TimeElapsed += Time.deltaTime;
        if (transform.position == m_targetPosition.position)
        {
            m_IsDoneMoving = true;

            isDone = true;
        }
        else
        {
            m_IsDoneMoving = false;
            
        }


        
        return isDone;
    }
}
