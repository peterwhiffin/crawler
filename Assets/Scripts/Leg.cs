using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class Leg : MonoBehaviour
{
    private float m_TimeElapsed;    
    private bool m_DoMove;

    [SerializeField] private Player m_Player;
    [SerializeField] private Transform m_TargetPosition;
    [SerializeField] private Vector2 m_Orientation;
    [SerializeField] private CrawlerSettings m_CrawlerSettings;
    [SerializeField] private LineRenderer m_LineRenderer;

    private List<Vector3> allRopeSections = new();
    public Vector2 Orientation { get { return m_Orientation; } }
    public Vector2 TargetPosition { get { return m_TargetPosition.position; } }

    public void Initialize(float moveRate)
    {
        m_TimeElapsed = 0f;
        m_DoMove = true;
    }

    private void Update()
    {
        if (m_TimeElapsed >= m_CrawlerSettings.LegMoveRate)
        {
            m_DoMove = false;
        }

        if (m_DoMove)
        {
            
            float time = m_TimeElapsed / m_CrawlerSettings.LegMoveRate;
            Vector3 target = m_TargetPosition.position + m_TargetPosition.up * m_CrawlerSettings.StepHeight * m_CrawlerSettings.StepCurve.Evaluate(time);
            transform.position = Vector3.Lerp(transform.position, target, time);
            m_TimeElapsed += Time.deltaTime;
        }
        else
        {
            transform.position = m_TargetPosition.position;
        }
               
    }

    public void SetTarget(Transform parent, Vector2 targetPosition, Vector2 targetNormal)
    {
        m_TargetPosition.SetParent(null);
        m_TargetPosition.up = targetNormal;
        m_TargetPosition.SetParent(parent);
        m_TargetPosition.position = targetPosition;
        
        transform.up = targetNormal;
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

    public void SnapPosition(Vector3 position)
    {
        transform.position = position;
    }

    private void LateUpdate()
    {
        DisplayRope();
    }

    private void DisplayRope()
    {
        float ropeWidth = 0.1f;

        m_LineRenderer.startWidth = ropeWidth;
        m_LineRenderer.endWidth = ropeWidth;

        Vector3[] pos = new Vector3[2] { transform.position, m_Player.transform.position };
        m_LineRenderer.positionCount = 2;
        m_LineRenderer.SetPositions(pos);
    }
}
