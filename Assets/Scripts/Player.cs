using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    public List<Leg> m_Legs = new List<Leg>();

    private Vector2 m_MoveInput;
    public Transform m_RestPosition;
    public float m_Speed;
    public float m_MaxDistanceFromLeg;
    public float m_MaxRayDistance;
    public LayerMask m_LegLayerMask;
    public float m_MinimumLegMoveDistance;
    public float m_LegMoveRate;
    public Rigidbody2D m_RigidBody;
    public float m_MaxLegDistance;
    public Dictionary<float, RaycastHit2D> m_Hits = new();
    public List<Leg> m_LegsWithPosition = new List<Leg>();
    public List<Leg> m_LegsWithoutPosition = new List<Leg>();
    public int m_LegIndex;
    public float m_SpringForce;
    public float m_DampForce;
    public float m_MaxSpringStretch;
    public float m_LegMoveThreshold;
    public float m_LegReach;
    public Transform testMax;
    public Transform testUp;
    public Transform testForward;
    public Vector2 m_LastInput;
    public Dictionary<RaycastHit2D, float> m_TempHits = new();
    public RaycastHit2D m_BestHit;
    public float m_PlayerPositionBias;

    private void Update()
    {
        Vector3 restPosition = Vector3.zero;

        foreach (Leg leg1 in m_Legs)
        {
            restPosition += leg1.m_targetPosition.position;
        }

        restPosition += transform.position;
        restPosition = restPosition / 5;
        m_RestPosition.position = restPosition;

        if (Vector3.Distance(transform.position, m_RestPosition.position) > m_LegMoveThreshold)
        {
            if (m_Legs[m_LegIndex].IsDoneMoving())
            {
                m_LegIndex++;

                if (m_LegIndex > m_Legs.Count - 1)
                {
                    m_LegIndex = 0;
                }

                bool hitFound = GetHits(m_Legs[m_LegIndex].m_Orientation.horizontal, m_Legs[m_LegIndex].m_Orientation.vertical);
                if (m_LegsWithPosition.Contains(m_Legs[m_LegIndex]))
                {
                    m_LegsWithPosition.Remove(m_Legs[m_LegIndex]);
                }

                if (hitFound)
                {
                    if (Vector3.Distance(m_Legs[m_LegIndex].transform.position, m_BestHit.point) > m_MinimumLegMoveDistance)
                    {
                        m_Legs[m_LegIndex].m_targetPosition.position = m_BestHit.point;
                        m_Legs[m_LegIndex].StartMove();
                    }
                    m_LegsWithPosition.Add(m_Legs[m_LegIndex]);
                    
                }
                else
                {
                    //if (m_LegsWithPosition.Count > 0)
                    //{
                    //    var newPosition = m_LegsWithPosition[0].transform.position;
                    //    float x = m_Legs[m_LegIndex].m_Orientation.horizontal;
                    //    float y = m_Legs[m_LegIndex].m_Orientation.vertical;
                    //    Vector3 origin = transform.position;
                    //    Vector3 max = origin + (transform.right * x * m_LegReach) + (transform.up * y * m_LegReach);

                    //    for (int i = 0; i < m_LegsWithPosition.Count; i++)
                    //    {
                    //        if (Vector3.Distance(m_LegsWithPosition[i].transform.position, max) < Vector3.Distance(newPosition, max))
                    //        {
                    //            newPosition = m_LegsWithPosition[i].m_targetPosition.position;
                    //        }
                    //    }

                    //    m_Legs[m_LegIndex].m_targetPosition.position = newPosition;
                    //}
                }

               // m_Legs[m_LegIndex].StartMove();
            }
        }
    }

    private bool GetHits(float x, float y)
    {
        bool hitFound = false;
        bool preferredHitFound = false;
        //Vector3 newPosition = transform.position;

        Vector3 origin = transform.position;
        Vector3 forward = origin + (transform.right * x * m_LegReach);
        Vector3 up = origin + (transform.up * y * m_LegReach);
        Vector3 max = origin + (transform.right * x * m_LegReach) + (transform.up * y * m_LegReach);
        Vector3 reverseXMax = origin + (transform.right * -x * m_LegReach) + (transform.up * y * m_LegReach);
        Vector3 reverseYMax = origin + (transform.right * x * m_LegReach) + (transform.up * -y * m_LegReach);
        Vector3 halfY = origin + (transform.up * y * (m_LegReach / 2f));
        Vector3 halfX = origin + (transform.right * x * (m_LegReach / 2f));
        Vector3 reverseY = origin + (transform.up * -y * m_LegReach);
        Vector3 reverseX = origin + (transform.right * -x * m_LegReach);

        //RaycastHit2D hitOriginToForward = Physics2D.Raycast(origin, forward - origin, Vector3.Distance(forward, origin), m_LegLayerMask);
        RaycastHit2D hitForwardToMax = Physics2D.Raycast(forward, max - forward, Vector3.Distance(max, forward), m_LegLayerMask);
        //RaycastHit2D hitOriginToUp = Physics2D.Raycast(origin, up - origin, Vector3.Distance(up, origin), m_LegLayerMask);
        RaycastHit2D hitUpToMax = Physics2D.Raycast(up, max - up, Vector3.Distance(max, up), m_LegLayerMask);
        RaycastHit2D hitOriginToMax = Physics2D.Raycast(origin, max - origin, Vector3.Distance(origin, max), m_LegLayerMask);
        //RaycastHit2D hitMaxToOrigin = Physics2D.Raycast(max, origin - max, Vector3.Distance(max, origin), m_LegLayerMask);
        //RaycastHit2D hitForwardToUp = Physics2D.Raycast(forward, up - forward, Vector3.Distance(forward, up), m_LegLayerMask);
        //RaycastHit2D hitUpToForward = Physics2D.Raycast(up, forward - up, Vector3.Distance(up, forward), m_LegLayerMask);
       
        RaycastHit2D hitMaxToForward = Physics2D.Raycast(max, forward - max, Vector3.Distance(max, forward), m_LegLayerMask);

        
        
        //Vector3 inputBias = new Vector3(m_LastInput.x, m_LastInput.y, 0f);

        //if(hitOriginToForward && hitOriginToForward.fraction != 0f)
        //{
        //    m_TempHits.Add(hitOriginToForward, Vector3.Distance(hitOriginToForward.point, origin));
        //}

        if (hitForwardToMax && hitForwardToMax.fraction != 0f)
        {
            m_TempHits.Add(hitForwardToMax, Vector3.Distance(hitForwardToMax.point, origin));
            preferredHitFound = true;
        }

        if (hitMaxToForward && hitMaxToForward.fraction != 0f)
        {
            m_TempHits.Add(hitMaxToForward, Vector3.Distance(hitMaxToForward.point, origin));
            preferredHitFound = true;
        }

        
        //if (hitOriginToUp && hitOriginToUp.fraction != 0f)
        //{
        //    m_TempHits.Add(hitOriginToUp, Vector3.Distance(hitOriginToUp.point, origin));
        //}

        if (hitUpToMax && hitUpToMax.fraction != 0f)
        {
            m_TempHits.Add(hitUpToMax, Vector3.Distance(hitUpToMax.point, origin));
            preferredHitFound = true;
        }

        if (hitOriginToMax && hitOriginToMax.fraction != 0f)
        {
            m_TempHits.Add(hitOriginToMax, Vector3.Distance(hitOriginToMax.point, origin));
            preferredHitFound = true;
        }


        //if (hitMaxToOrigin && hitMaxToOrigin.fraction != 0f)
        //{
        //    m_TempHits.Add(hitMaxToOrigin, Vector3.Distance(hitMaxToOrigin.point, origin));
        //}

        //if (hitForwardToUp && hitForwardToUp.fraction != 0f)
        //{
        //    m_TempHits.Add(hitForwardToUp, Vector3.Distance(hitForwardToUp.point, origin));
        //}

        //if (hitUpToForward && hitUpToForward.fraction != 0f)
        //{
        //    m_TempHits.Add(hitUpToForward, Vector3.Distance(hitUpToForward.point, origin));
        //}

        if (!preferredHitFound)
        {
            RaycastHit2D hitMaxToReverseYMax = Physics2D.Raycast(max, reverseYMax - max, Vector3.Distance(max, reverseYMax), m_LegLayerMask);
            RaycastHit2D hitUpToReverse = Physics2D.Raycast(up, reverseXMax - up, Vector3.Distance(up, reverseXMax), m_LegLayerMask);
            RaycastHit2D hitHalfYToReverse = Physics2D.Raycast(halfY, reverseY - halfY, Vector3.Distance(reverseY, halfY), m_LegLayerMask);
            RaycastHit2D hitHalfXToReverse = Physics2D.Raycast(halfX, reverseX - halfX, Vector3.Distance(reverseX, halfX), m_LegLayerMask);

            if (hitMaxToReverseYMax && hitMaxToReverseYMax.fraction != 0f)
            {
                m_TempHits.Add(hitMaxToReverseYMax, Vector3.Distance(hitMaxToReverseYMax.point, origin));
            }

            if (hitUpToReverse && hitUpToReverse.fraction != 0f)
            {
                m_TempHits.Add(hitUpToReverse, Vector3.Distance(hitUpToReverse.point, origin));
            }

            if (hitHalfYToReverse && hitHalfYToReverse.fraction != 0f)
            {
                m_TempHits.Add(hitHalfYToReverse, Vector3.Distance(hitHalfYToReverse.point, origin));
            }

            if (hitHalfXToReverse && hitHalfXToReverse.fraction != 0f)
            {
                m_TempHits.Add(hitHalfXToReverse, Vector3.Distance(hitHalfXToReverse.point, origin));
            }


            float closest = 100f;

            foreach (var hit in m_TempHits)
            {
                if (hit.Value < closest)
                {
                    closest = hit.Value;
                    m_BestHit = hit.Key;
                    hitFound = true;
                }
            }

        }
        else
        {
            float farthestHit = -1f;

            foreach (var hit in m_TempHits)
            {
                if (hit.Value > farthestHit)
                {
                    farthestHit = hit.Value;
                    m_BestHit = hit.Key;
                    hitFound = true;
                }
            }
        }

        m_TempHits.Clear();
        return hitFound;
    }

    private void FixedUpdate()
    {
        if ((transform.position - m_RestPosition.position).magnitude < m_MaxSpringStretch)
        {
            m_RigidBody.AddForce(m_MoveInput.normalized * m_Speed * Time.fixedDeltaTime);

        }       

        if (m_MoveInput == Vector2.zero)
        {
            m_RigidBody.AddForce((m_RestPosition.position - transform.position) * m_SpringForce * Time.fixedDeltaTime);
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        m_MoveInput = context.ReadValue<Vector2>();

        if (m_MoveInput != Vector2.zero)
        {
            m_LastInput = m_MoveInput;
        }
    }
}
