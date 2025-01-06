using System.Collections;
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
    [SerializeField] private WeaponSettings m_Settings;
    [SerializeField] private AudioSource m_AudioSource;
    private bool m_IsAudioPlaying = false;
    public override Transform FirePosition { get { return m_FirePosition; } }
    private float m_AudioPlayTime;
    private Coroutine m_AudioEndRoutine;
    private void Start()
    {
        m_LastFireTime = Time.time - m_FireRate;
        m_AudioSource.clip = m_Settings.FireSound;
    }

    public override bool StartPrimaryAttack()
    {
        if(CheckFireRate())
        {
            Projectile prefab = Instantiate(m_ProjectilePrefab);
            var rotation = m_FirePosition.eulerAngles;

            rotation.z += Random.Range(-m_Settings.Accuracy, m_Settings.Accuracy);
            prefab.Fire(m_FirePosition.position, Quaternion.Euler(rotation), m_Settings.ProjectileSpeed, m_Settings.Damage, m_Settings.ProjectileRadius);
            
            m_LastFireTime = Time.time;

            if (m_Settings.IsAudioOneShot)
            {
                m_AudioSource.PlayOneShot(m_AudioSource.clip);
            }
            else
            {

                if (!m_AudioSource.isPlaying)
                {
                    m_AudioPlayTime = Time.time;
                    m_AudioSource.Play();
                }
            }           
        }

        return base.StartPrimaryAttack();
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
        if (m_AudioSource.isPlaying)
        {
            if (Time.time - m_AudioPlayTime > m_Settings.MinimumAudioTime)
            {
                m_AudioSource.Stop();
            }
            else
            {
                if(m_AudioEndRoutine != null)
                {
                    StopCoroutine(m_AudioEndRoutine);
                }

                StartCoroutine(EndAudioAfterTime());
            }
        }
    }

    private IEnumerator EndAudioAfterTime()
    {
        while(Time.time - m_AudioPlayTime < m_Settings.MinimumAudioTime)
        {
            yield return null;
        }

        m_AudioSource.Stop();
    }
}
