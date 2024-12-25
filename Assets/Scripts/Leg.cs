using System;
using Unity.VisualScripting;
using UnityEngine;

public class Leg : MonoBehaviour
{
    private float m_TimeElapsed;    
    private float m_MoveRate;
    
    private bool m_DoMove;

    [SerializeField] private Transform m_TargetPosition;
    [SerializeField] private Vector2 m_Orientation;
    
    public Vector2 Orientation { get { return m_Orientation; } }
    public Vector2 TargetPosition { get { return m_TargetPosition.position; } }

    public void Initialize(float moveRate)
    {
        m_TimeElapsed = 0f;
        m_DoMove = true;
        m_MoveRate = moveRate;
    }

    private void Update()
    {
        if (m_DoMove)
        {
            m_TimeElapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, m_TargetPosition.position, m_TimeElapsed / m_MoveRate);

            if (transform.position == m_TargetPosition.position)
            {
                m_DoMove = false;
            }
        }
        else
        {
            transform.position = m_TargetPosition.position;
        }
    }

    public void StartMove(Transform parent, Vector3 targetPosition)
    {
        m_TargetPosition.SetParent(parent);
        m_TargetPosition.position = targetPosition;
        m_TimeElapsed = 0f;
        m_DoMove = true;
    }

    public bool IsDoneMoving()
    {
        return !m_DoMove;
    }
}
