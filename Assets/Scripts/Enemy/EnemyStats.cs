using System;
using UnityEngine;

public class EnemyStats : Stats
{
    [SerializeField] private Enemy m_Enemy;

    private void Start()
    {
        m_CurrentHealth = m_Enemy.Settings.MaxHealth;
        m_Enemy.HitBox.OnHit += ApplyDamage;
    }

    private void OnDestroy()
    {
        m_Enemy.HitBox.OnHit -= ApplyDamage;
    }
}
