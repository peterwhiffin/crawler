using System;
using UnityEngine;

public class EnemyStats : Stats
{
    [SerializeField] private Enemy m_Enemy;
    [SerializeField] private int m_GrappledLayer;

    private void Start()
    {
        m_CurrentHealth = m_Enemy.Settings.MaxHealth;
        m_Enemy.HitBox.OnHit += ApplyDamage;
        m_Enemy.HitBox.ReeledIn += OnReeledIn;
        m_Enemy.HitBox.OnGrappleHit += OnGrappleHit;
    }

    private void OnDestroy()
    {
        m_Enemy.HitBox.OnHit -= ApplyDamage;
        m_Enemy.HitBox.ReeledIn -= OnReeledIn;
        m_Enemy.HitBox.OnGrappleHit -= OnGrappleHit;
    }

    private void OnReeledIn()
    {
        ApplyDamage(m_CurrentHealth + 1f);
    }

    private void OnGrappleHit()
    {
        gameObject.layer = m_GrappledLayer;
    }
}
