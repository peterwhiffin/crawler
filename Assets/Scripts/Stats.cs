using System;
using UnityEngine;

public class Stats : MonoBehaviour
{
    protected float m_CurrentHealth = 100f;
    protected bool m_IsAlive = true;
    public Action<float> Damaged = delegate { };
    public Action Died = delegate { };  
    public Action Healed = delegate { };

    public float CurrentHealth {  get { return m_CurrentHealth; } }

    public void ApplyDamage(float damage)
    {
        if (!m_IsAlive)
        {
            return;
        }

        m_CurrentHealth -= damage;
        Damaged.Invoke(damage);

        if (m_CurrentHealth <= 0f)
        {
            m_IsAlive = false;
            Died.Invoke();
        }
    }

    public virtual void ApplyHeal(float health)
    {
        if (!m_IsAlive)
        {
            return;
        }
    }
}
