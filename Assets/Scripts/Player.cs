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
    public RaycastHit2D m_ClosestHit;

    private void Update()
    {
        Vector3 restPosition = Vector3.zero;

        foreach (Leg leg1 in m_Legs)
        {
            restPosition += leg1.transform.position;
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
                    if (Vector3.Distance(m_Legs[m_LegIndex].transform.position, m_ClosestHit.point) > m_MinimumLegMoveDistance)
                    {

                        m_Legs[m_LegIndex].m_targetPosition.position = m_ClosestHit.point;
                    }
                    m_LegsWithPosition.Add(m_Legs[m_LegIndex]);
                }
                else
                {
                    if (m_LegsWithPosition.Count > 0)
                    {
                        var newPosition = m_LegsWithPosition[0].transform.position;
                        float x = m_Legs[m_LegIndex].m_Orientation.horizontal;
                        float y = m_Legs[m_LegIndex].m_Orientation.vertical;

                        Vector3 origin = transform.position;

                        Vector3 max = origin + (transform.right * x * m_LegReach) + (transform.up * y * m_LegReach);



                        for (int i = 0; i < m_LegsWithPosition.Count; i++)
                        {
                            if (Vector3.Distance(m_LegsWithPosition[i].transform.position, max) < Vector3.Distance(newPosition, max))
                            {
                                newPosition = m_LegsWithPosition[i].m_targetPosition.position;
                            }
                        }

                        m_Legs[m_LegIndex].m_targetPosition.position = newPosition;
                        //m_LegsWithPosition.Add(m_Legs[m_LegIndex]);
                    }
                }


                m_Legs[m_LegIndex].StartMove();

            }
            else
            {
                Debug.Log("current leg not done moving");
            }
        }
        else
        {
            Debug.Log("player not past move threshold");
        }
        //foreach (Leg leg in m_Legs)
        //{
        //    if (!leg.m_IsDoneMoving)
        //        continue;

        //    var xPos = new Vector3(leg.m_RayPosition.position.x, transform.position.y, 0f);
        //    var yPos = new Vector3(transform.position.x, leg.m_RayPosition.position.y, 0f);
        //    var xDir = leg.m_RayPosition.position - xPos;
        //    var yDir = leg.m_RayPosition.position - yPos;

        //    RaycastHit2D legHitXPos = Physics2D.Raycast(xPos, xDir, Vector3.Distance(xPos, leg.m_RayPosition.position), m_LegLayerMask);
        //    RaycastHit2D legHitYPos = Physics2D.Raycast(yPos, yDir, Vector3.Distance(yPos, leg.m_RayPosition.position), m_LegLayerMask);
        //    RaycastHit2D legHit = Physics2D.Raycast(transform.position, leg.m_RayPosition.position - transform.position, Vector3.Distance(transform.position, leg.m_RayPosition.position), m_LegLayerMask);
        //    RaycastHit2D reverseLegHit = Physics2D.Raycast(leg.m_RayPosition.position, transform.position - leg.m_RayPosition.position, Vector3.Distance(transform.position, leg.m_RayPosition.position), m_LegLayerMask);

        //    if (legHitXPos && legHitXPos.fraction != 0f) 
        //    {
        //        if (!m_Hits.ContainsKey(Vector3.Distance(transform.position, legHitXPos.point)))
        //            m_Hits.Add(Vector3.Distance(transform.position, legHitXPos.point), legHitXPos);
        //    }

        //    if (legHitYPos && legHitYPos.fraction != 0f)
        //    {
        //        if (!m_Hits.ContainsKey(Vector3.Distance(transform.position, legHitYPos.point)))
        //            m_Hits.Add(Vector3.Distance(transform.position, legHitYPos.point), legHitYPos);
        //    }

        //    if (legHit && legHit.fraction != 0f)
        //    {
        //        if (!m_Hits.ContainsKey(Vector3.Distance(transform.position, legHit.point)))
        //            m_Hits.Add(Vector3.Distance(transform.position, legHit.point), legHit);
        //    }

        //    if (reverseLegHit && reverseLegHit.fraction != 0f)
        //    {
        //        if (!m_Hits.ContainsKey(Vector3.Distance(transform.position, reverseLegHit.point)))
        //            m_Hits.Add(Vector3.Distance(transform.position, reverseLegHit.point), reverseLegHit);
        //    }

        //    float hitKey = -1f;

        //    foreach (var hit in m_Hits)
        //    {
        //        if (hit.Key > hitKey)
        //        {
        //            hitKey = hit.Key;
        //        }
        //    }

        //    if (hitKey >= 0f)
        //    {
        //        if (Vector3.Distance(leg.transform.position, m_Hits[hitKey].point) > m_MinimumLegMoveDistance)
        //        {
        //            leg.m_targetPosition.position = m_Hits[hitKey].point;
        //            leg.m_targetPosition.SetParent(m_Hits[hitKey].transform, true);
        //        }
        //        m_LegsWithPosition.Add(leg);
        //        leg.m_IsHosted = false;
        //    }
        //    else
        //    {
        //        m_LegsWithoutPosition.Add(leg);
        //    }

        //    m_Hits.Clear();
        //}

        //if (m_LegsWithPosition.Count > 0)
        //{
        //    foreach (Leg lostLeg in m_LegsWithoutPosition)
        //    {


        //        if (lostLeg.m_IsHosted)
        //        {
        //            if (Vector3.Distance(lostLeg.transform.position, transform.position) < m_MaxLegDistance)
        //            {
        //                continue;
        //            }
        //        }


        //        lostLeg.m_IsHosted = true;
        //        Leg hostLeg = m_LegsWithPosition[0];

        //        if (m_LegsWithPosition.Count > 1)
        //        {
        //            for (int i = 1; i < m_LegsWithPosition.Count; i++)
        //            {
        //                if (Vector3.Distance(hostLeg.transform.position, lostLeg.m_RayPosition.position) > Vector3.Distance(m_LegsWithPosition[i].transform.position, lostLeg.m_RayPosition.position))
        //                {
        //                    hostLeg = m_LegsWithPosition[i];
        //                }
        //            }
        //        }

        //        RaycastHit2D reverseHit = Physics2D.Raycast(lostLeg.m_RayPosition.position, hostLeg.transform.position - lostLeg.m_RayPosition.position, Vector3.Distance(hostLeg.transform.position, lostLeg.m_RayPosition.position), m_LegLayerMask);

        //        if (reverseHit && reverseHit.fraction != 0)
        //        {
        //            lostLeg.m_targetPosition.position = reverseHit.point;
        //            lostLeg.m_targetPosition.SetParent(reverseHit.transform, true);
        //        }
        //        else
        //        {
        //            lostLeg.m_targetPosition.position = new Vector3(hostLeg.m_targetPosition.position.x, hostLeg.m_targetPosition.position.y, 0f);
        //            lostLeg.m_targetPosition.SetParent(hostLeg.m_targetPosition.parent, true);
        //        }

        //    }
        //}

        //m_LegsWithPosition.Clear();
        //m_LegsWithoutPosition.Clear();


        //if (Vector3.Distance(transform.position, m_RestPosition.position) > m_LegMoveThreshold)
        //{
        //    if (m_Legs[m_LegIndex].IsDoneMoving())
        //    {
        //        m_LegIndex++;

        //        if (m_LegIndex > m_Legs.Count - 1)
        //        {
        //            m_LegIndex = 0;
        //        }

        //        m_Legs[m_LegIndex].m_NeedsToMove = true;
        //    }
        //}
    }

    private bool GetHits(float x, float y)
    {
        bool hitFound = false;

        Vector3 newPosition = transform.position;

        Vector3 origin = transform.position;
        Vector3 forward = origin + (transform.right * x * m_LegReach);
        Vector3 up = origin + (transform.up * y * m_LegReach);
        Vector3 max = origin + (transform.right * x * m_LegReach) + (transform.up * y * m_LegReach);

        RaycastHit2D hitOriginToForward = Physics2D.Raycast(origin, forward - origin, Vector3.Distance(forward, origin), m_LegLayerMask);
        RaycastHit2D hitForwardToMax = Physics2D.Raycast(forward, max - forward, Vector3.Distance(max, forward), m_LegLayerMask);
        RaycastHit2D hitOriginToUp = Physics2D.Raycast(origin, up - origin, Vector3.Distance(up, origin), m_LegLayerMask);
        RaycastHit2D hitUpToMax = Physics2D.Raycast(up, max - up, Vector3.Distance(max, up), m_LegLayerMask);
        RaycastHit2D hitOriginToMax = Physics2D.Raycast(origin, max - origin, Vector3.Distance(origin, max), m_LegLayerMask);
        //RaycastHit2D hitMaxToOrigin = Physics2D.Raycast(max, origin - max, Vector3.Distance(max, origin), m_LegLayerMask);
        RaycastHit2D hitForwardToUp = Physics2D.Raycast(forward, up - forward, Vector3.Distance(forward, up), m_LegLayerMask);
        RaycastHit2D hitUpToForward = Physics2D.Raycast(up, forward - up, Vector3.Distance(up, forward), m_LegLayerMask);

        Vector3 inputBias = new Vector3(m_LastInput.x, m_LastInput.y, 0f);

        if(hitOriginToForward && hitOriginToForward.fraction != 0f)
        {
            m_TempHits.Add(hitOriginToForward, Vector3.Distance(hitOriginToForward.point, origin));
        }

        if (hitForwardToMax && hitForwardToMax.fraction != 0f)
        {
            m_TempHits.Add(hitForwardToMax, Vector3.Distance(hitForwardToMax.point, origin));
        }

        if (hitOriginToUp && hitOriginToUp.fraction != 0f)
        {
            m_TempHits.Add(hitOriginToUp, Vector3.Distance(hitOriginToUp.point, origin));
        }

        if (hitUpToMax && hitUpToMax.fraction != 0f)
        {
            m_TempHits.Add(hitUpToMax, Vector3.Distance(hitUpToMax.point, origin));
        }

        if (hitOriginToMax && hitOriginToMax.fraction != 0f)
        {
            m_TempHits.Add(hitOriginToMax, Vector3.Distance(hitOriginToMax.point, origin));
        }

        //if (hitMaxToOrigin && hitMaxToOrigin.fraction != 0f)
        //{
        //    m_TempHits.Add(hitMaxToOrigin, Vector3.Distance(hitMaxToOrigin.point, origin));
        //}

        if (hitForwardToUp && hitForwardToUp.fraction != 0f)
        {
            m_TempHits.Add(hitForwardToUp, Vector3.Distance(hitForwardToUp.point, origin));
        }

        if (hitUpToForward && hitUpToForward.fraction != 0f)
        {
            m_TempHits.Add(hitUpToForward, Vector3.Distance(hitUpToForward.point, origin));
        }

        float farthestHit = -1f;

        foreach(var hit in m_TempHits)
        {
            if(hit.Value > farthestHit)
            {
                farthestHit = hit.Value;
                m_ClosestHit = hit.Key;
                hitFound = true;
            }
        }

        //if(m_ClosestHit == hitMaxToOrigin)
        //{
        //    if(hitOriginToMax && hitOriginToMax.fraction != 0)
        //    {
        //        m_ClosestHit = hitOriginToMax;
        //    }
        //}

        m_TempHits.Clear();
        return hitFound;
    }

    private void FixedUpdate()
    {
        if ((transform.position - m_RestPosition.position).magnitude < m_MaxSpringStretch)
        {
            m_RigidBody.AddForce(m_MoveInput.normalized * m_Speed * Time.fixedDeltaTime);
        }

        if(m_MoveInput == Vector2.zero)
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
