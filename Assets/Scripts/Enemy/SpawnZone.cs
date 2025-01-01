using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] private Enemy m_EnemyPrefab;

    [SerializeField] private Transform m_SpawnPosition;
    [SerializeField] List<Transform> m_PatrolPositions;
    [SerializeField] private float m_RespawnTime;
    [SerializeField] private Player m_Player;
    [SerializeField] private Slider m_HealthbarPrefab;
    [SerializeField] private Transform m_HealthbarParent;

    private Enemy m_CurrentEnemy;
    public List<Transform> PatrolPositions { get { return m_PatrolPositions; } }

    private void Start()
    {
        SpawnEnemy();
    }

    private void OnEnemyDied()
    {
        m_CurrentEnemy.Stats.Died -= OnEnemyDied;
        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        float time = Time.time;

        while(Time.time - time < m_RespawnTime || Vector3.Distance(transform.position, m_Player.transform.position) < 30f)
        {
            yield return null;
        }

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        Slider healthBar = Instantiate(m_HealthbarPrefab, m_HealthbarParent);
        Enemy enemy = Instantiate(m_EnemyPrefab);
        enemy.transform.position = m_SpawnPosition.position;
        enemy.Initialize(m_Player, healthBar, this);
        enemy.Stats.Died += OnEnemyDied;
        m_CurrentEnemy = enemy;
    }
}
