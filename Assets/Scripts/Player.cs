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
    private void Update()
    {

        Vector3 restPosition = Vector3.zero;

        foreach(Leg leg1 in m_Legs)
        {
            restPosition += leg1.transform.position;
        }

        restPosition = restPosition / 4;

        m_RestPosition.position = restPosition;

        foreach (Leg leg in m_Legs)
        {
            if (!leg.m_IsDoneMoving)
                continue;

            var xPos = new Vector3(leg.m_RayPosition.position.x, transform.position.y, 0f);
            var yPos = new Vector3(transform.position.x, leg.m_RayPosition.position.y, 0f);
            var xDir = leg.m_RayPosition.position - xPos;
            var yDir = leg.m_RayPosition.position - yPos;
            


            RaycastHit2D legHitXPos = Physics2D.Raycast(xPos, xDir, Vector3.Distance(xPos, leg.m_RayPosition.position), m_LegLayerMask);
            RaycastHit2D legHitYPos = Physics2D.Raycast(yPos, yDir, Vector3.Distance(yPos, leg.m_RayPosition.position), m_LegLayerMask);
            RaycastHit2D legHit = Physics2D.Raycast(transform.position, leg.m_RayPosition.position - transform.position, Vector3.Distance(transform.position, leg.m_RayPosition.position), m_LegLayerMask);
            RaycastHit2D reverseLegHit = Physics2D.Raycast(leg.m_RayPosition.position, transform.position - leg.m_RayPosition.position, Vector3.Distance(transform.position, leg.m_RayPosition.position), m_LegLayerMask);

            if (legHitXPos && legHitXPos.fraction != 0f)
            {
                if (!m_Hits.ContainsKey(Vector3.Distance(transform.position, legHitXPos.point)))
                    m_Hits.Add(Vector3.Distance(transform.position, legHitXPos.point), legHitXPos);
            }

            if (legHitYPos && legHitYPos.fraction != 0f)
            {
                if (!m_Hits.ContainsKey(Vector3.Distance(transform.position, legHitYPos.point)))
                    m_Hits.Add(Vector3.Distance(transform.position, legHitYPos.point), legHitYPos);
            }

            if (legHit && legHit.fraction != 0f)
            {
                if (!m_Hits.ContainsKey(Vector3.Distance(transform.position, legHit.point)))
                    m_Hits.Add(Vector3.Distance(transform.position, legHit.point), legHit);
            }

            if (reverseLegHit && reverseLegHit.fraction != 0f)
            {
                if (!m_Hits.ContainsKey(Vector3.Distance(transform.position, reverseLegHit.point)))
                    m_Hits.Add(Vector3.Distance(transform.position, reverseLegHit.point), reverseLegHit);
            }

            float hitKey = -1f;

            foreach (var hit in m_Hits)
            {
                if (hit.Key > hitKey)
                {
                    hitKey = hit.Key;
                }
            }

            if (hitKey >= 0f)
            {
                if (Vector3.Distance(leg.transform.position, m_Hits[hitKey].point) > m_MinimumLegMoveDistance)
                {
                    leg.m_targetPosition.position = m_Hits[hitKey].point;
                    leg.m_targetPosition.SetParent(m_Hits[hitKey].transform, true);
                }
                m_LegsWithPosition.Add(leg);
                leg.m_IsHosted = false;
            }
            else
            {
                m_LegsWithoutPosition.Add(leg);
            }

            m_Hits.Clear();
        }

        if (m_LegsWithPosition.Count > 0)
        {
            foreach (Leg lostLeg in m_LegsWithoutPosition)
            {


                if (lostLeg.m_IsHosted)
                {
                    if (Vector3.Distance(lostLeg.transform.position, transform.position) < m_MaxLegDistance)
                    {
                        continue;
                    }
                }


                lostLeg.m_IsHosted = true;
                Leg hostLeg = m_LegsWithPosition[0];

                if (m_LegsWithPosition.Count > 1)
                {
                    for (int i = 1; i < m_LegsWithPosition.Count; i++)
                    {
                        if (Vector3.Distance(hostLeg.transform.position, lostLeg.m_RayPosition.position) > Vector3.Distance(m_LegsWithPosition[i].transform.position, lostLeg.m_RayPosition.position))
                        {
                            hostLeg = m_LegsWithPosition[i];
                        }
                    }
                }

                RaycastHit2D reverseHit = Physics2D.Raycast(lostLeg.m_RayPosition.position, hostLeg.transform.position - lostLeg.m_RayPosition.position, Vector3.Distance(hostLeg.transform.position, lostLeg.m_RayPosition.position), m_LegLayerMask);

                if (reverseHit && reverseHit.fraction != 0)
                {
                    lostLeg.m_targetPosition.position = reverseHit.point;
                    lostLeg.m_targetPosition.SetParent(reverseHit.transform, true);
                }
                else
                {
                    lostLeg.m_targetPosition.position = new Vector3(hostLeg.m_targetPosition.position.x, hostLeg.m_targetPosition.position.y, 0f);
                    lostLeg.m_targetPosition.SetParent(hostLeg.m_targetPosition.parent, true);
                }
                //else if (Mathf.Abs(hostLeg.transform.position.x - lostLeg.transform.position.x) > Mathf.Abs(hostLeg.transform.position.y - lostLeg.transform.position.y))
                //{
                //    lostLeg.m_targetPosition = new Vector3(hostLeg.m_targetPosition.x, lostLeg.m_targetPosition.y, 0f);
                //}
                //else
                //{
                //    lostLeg.m_targetPosition = new Vector3(lostLeg.transform.position.x, hostLeg.m_targetPosition.y, 0f);
                //}

                //lostLeg.m_targetPosition = hostLeg.transform.position;
            }
        }

        m_LegsWithPosition.Clear();
        m_LegsWithoutPosition.Clear();



        if (m_Legs[m_LegIndex].MoveLeg())
        {
            m_LegIndex++;

            if(m_LegIndex > m_Legs.Count - 1)
            {
                m_LegIndex = 0;
            }

            m_Legs[m_LegIndex].m_NeedsToMove = true;
        }
    }

    private void FixedUpdate()
    {
        if ((transform.position - m_RestPosition.position).magnitude < m_MaxSpringStretch)
        {
            m_RigidBody.AddForce(m_MoveInput.normalized * m_Speed * Time.fixedDeltaTime);
        }

        if(m_MoveInput.x == 0f)
        {
            m_RigidBody.AddForce((new Vector3(m_RestPosition.position.x, transform.position.y, transform.position.z) - transform.position) * m_SpringForce * Time.fixedDeltaTime);
        }

        if(m_MoveInput.y == 0f)
        {
            m_RigidBody.AddForce((new Vector3(transform.position.x, m_RestPosition.position.y, transform.position.z) - transform.position) * m_SpringForce * Time.fixedDeltaTime);
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
       
            m_MoveInput = context.ReadValue<Vector2>();
        
    }
}
