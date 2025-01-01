using System.Collections.Generic;
using UnityEngine;

public class EnemyMotor : MonoBehaviour
{
    [SerializeField] private Enemy m_Enemy;
    [SerializeField] private SpawnZone m_SpawnZone;
    private List<Vector2> m_PlayerPositionHistory = new();
    private int m_PatrolIndex;
    [SerializeField] private EnemySettings m_Settings;

    public void Initialize(SpawnZone zone)
    {
        m_SpawnZone = zone;
    }

    public void PursuePlayer()
    {
        Vector2 newPosition = transform.position;
        newPosition = Vector2.MoveTowards(newPosition, m_Enemy.Player.transform.position, m_Settings.PursueSpeed * Time.deltaTime);
        transform.position = newPosition;

        //transform.up = m_Enemy.Player.transform.position - transform.position;
    }

    private void Update()
    {
        //Debug.Log("enemy dot: " + Vector2.Dot(transform.position, m_Enemy.transform.right));
    }

    public void SetNextPatrolTarget()
    {
        m_PatrolIndex++;

        if(m_PatrolIndex == m_SpawnZone.PatrolPositions.Count)
        {
            m_PatrolIndex = 0;
        }
    }

    public bool IsAtAttackDistance()
    {
        return Vector2.Distance(transform.position, m_Enemy.Player.transform.position) <= m_Settings.AttackDistance;
    }

    public void Patrol()
    {
        Vector2 newPosition = transform.position;
        newPosition = Vector2.MoveTowards(newPosition, m_SpawnZone.PatrolPositions[m_PatrolIndex].position, m_Settings.MoveSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    public bool IsAtPatrolGoal()
    {
        return Vector2.Distance(transform.position, m_SpawnZone.PatrolPositions[m_PatrolIndex].position) <= .2f;
    }

    public void CachePlayerPosition(Vector2 position)
    {
        m_PlayerPositionHistory.Add(position);

        if(m_PlayerPositionHistory.Count > 5)
        {
            m_PlayerPositionHistory.Remove(m_PlayerPositionHistory[0]);
        }
    }

    public void ClearPlayerPositionHistory()
    {
        m_PlayerPositionHistory.Clear();
    }

    public bool CanSeePlayer()
    {
        bool canSee = false;

        if (Vector2.Angle(m_Enemy.transform.forward, m_Enemy.Player.transform.position) < m_Enemy.Settings.FieldOfView)
        {
            canSee = PlayerWithinView();
        }

        return canSee;
    }

    public bool PlayerWithinAttackRange()
    {
        return Vector2.Distance(transform.position, m_Enemy.Player.transform.position) <= m_Settings.AttackDistance;
    }

    public bool PlayerWithinView()
    {
        bool canSee = false;
        RaycastHit2D hit = Physics2D.Raycast(m_Enemy.transform.position, m_Enemy.Player.transform.position - m_Enemy.transform.position, m_Enemy.Settings.SightDistance, m_Enemy.Settings.EnvironmentMask);
        
        if (hit && hit.collider.CompareTag("Player"))
        {
            canSee = true;
        }

        return canSee;
    }
}
