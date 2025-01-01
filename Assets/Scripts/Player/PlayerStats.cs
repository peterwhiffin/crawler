using System;
using UnityEngine;

public class PlayerStats : Stats
{
    [SerializeField] private Player m_Player;
    public bool IsAlive { get { return m_IsAlive; } }

    private void Start()
    {
        m_CurrentHealth = m_Player.CrawlerSettings.MaxHealth;
        m_Player.HitBox.OnHit += ApplyDamage;
    }

    private void OnDestroy()
    {
        m_Player.HitBox.OnHit -= ApplyDamage;
    }

    public void ResetPlayer()
    {
        m_IsAlive = true;
        m_CurrentHealth = m_Player.CrawlerSettings.MaxHealth;
    }

    public override void ApplyHeal(float health)
    {
        base.ApplyHeal(health);

        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth + health, 0f, m_Player.CrawlerSettings.MaxHealth);
        Healed.Invoke();
    }
}
