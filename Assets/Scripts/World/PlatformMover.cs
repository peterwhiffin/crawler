using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] private List<Transform> m_MovePositions;
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] private float m_WaitTime;
    [SerializeField] private Transform m_PositionsParent;

    private int m_CurrentIndex;
    private bool m_TargetReached;

    private void Start()
    {
        m_PositionsParent.SetParent(null);
        transform.position = m_MovePositions[0].position;
    }

    private void Update()
    {
        if (!m_TargetReached)
        {
            MoveTowardTargetPosition();
        }
    }

    private void MoveTowardTargetPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_MovePositions[m_CurrentIndex].position, m_MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, m_MovePositions[m_CurrentIndex].position) < .01f)
        {
            m_TargetReached = true;
            StopAllCoroutines();
            StartCoroutine(WaitForTime());
        }
    }

    private void SetNextIndex()
    {
        m_CurrentIndex++;

        if(m_CurrentIndex >= m_MovePositions.Count)
        {
            m_CurrentIndex = 0;
        }
    }

    private IEnumerator WaitForTime()
    {
        float timer = 0f;

        while(timer < m_WaitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        SetNextIndex();
        m_TargetReached = false;
    }
}
