using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private float m_CurrentHealth = 100f;
    private bool m_IsAlive = true;

    [SerializeField] private Player m_Player;

    public void ApplyDamage(float damage)
    {
        m_CurrentHealth -= damage;

        if(m_CurrentHealth <= 0f)
        {
            m_IsAlive = false;
        }

    }
}
