using System;
using UnityEngine;

public class Leg : MonoBehaviour
{
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
    public LegOrientation m_Orientation;
    private float m_TimeElapsed;
    private bool m_DoMove;

    private void Start()
    {
        m_IsDoneMoving = false;
        m_TimeElapsed = 0f;
        m_DoMove = true;
    }

    private void Update()
    {
        if (m_DoMove)
        {
            transform.position = Vector3.Lerp(transform.position, m_targetPosition.position, m_TimeElapsed / m_Player.m_LegMoveRate);

            m_TimeElapsed += Time.deltaTime;

            if (transform.position == m_targetPosition.position)
            {
                m_DoMove = false;
            }
        }
    }

    public void StartMove()
    {
        m_TimeElapsed = 0f;
        m_DoMove = true;
    }

    public bool IsDoneMoving()
    {
        return !m_DoMove;
    }
}
