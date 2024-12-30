using UnityEngine;

public class Gun : Equipment
{
    private float m_LastFireTime;

    [SerializeField] private float m_ProjectileSpeed;
    [SerializeField] private float m_FireRate;
    [SerializeField] private float m_Damage;
    [SerializeField] private Projectile m_ProjectilePrefab;
    [SerializeField] private Transform m_FirePosition;
    [SerializeField] private float m_ProjectileRadius;
    [SerializeField] private float m_Accuracy;

    
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
            //m_FirePosition.rotation = Quaternion.identity;
            //m_FirePosition.Rotate(new Vector3(0f, 0f, Random.Range(-m_Accuracy, m_Accuracy)));
            var rotation = m_FirePosition.eulerAngles;

            rotation.z += Random.Range(-m_Accuracy, m_Accuracy);
            prefab.Fire(m_FirePosition.position, Quaternion.Euler(rotation), m_ProjectileSpeed, m_Damage, m_ProjectileRadius);
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
