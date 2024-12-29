using UnityEngine;

public class Gun : Equipment
{
    private float m_LastFireTime;

    [SerializeField] private float m_ProjectileSpeed;
    [SerializeField] private float m_FireRate;
    [SerializeField] private Projectile m_ProjectilePrefab;
    [SerializeField] private Transform m_FirePosition;
    public override Transform FirePosition { get { return m_FirePosition; } }

    private void Start()
    {
        m_LastFireTime = Time.time - m_FireRate;
    }

    public override void StartPrimaryAttack()
    {
        if(CheckFireRate())
        {
            Projectile prefab = Instantiate(m_ProjectilePrefab);
            prefab.Fire(m_FirePosition.position, m_FirePosition.rotation, m_ProjectileSpeed);
            m_LastFireTime = Time.time;
        }
    }

    private bool CheckFireRate()
    {
        if(Time.time - m_LastFireTime > m_FireRate)
        {
            return true;
        }

        return false;
    }

    public override void CancelPrimaryAttack()
    {
        
    }
}
