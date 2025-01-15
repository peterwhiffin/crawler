using UnityEngine;
using System.Collections;

public class MeleeWeapon : Equipment
{
    private float m_LastFireTime;
    private float m_AudioPlayTime;
    private Coroutine m_AudioEndRoutine;

    [SerializeField] private MeleeSettings m_Settings;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private Player m_Player;
    [SerializeField] private Transform m_FirePosition;
    [SerializeField] private Transform M_FireEffectPosition;

    private void Start()
    {
        m_LastFireTime = Time.time - m_Settings.FireRate;
        m_AudioSource.clip = m_Settings.FireSound;
    }

    public override void Activate()
    {
        base.Activate();
        m_AudioSource.clip = m_Settings.FireSound;
    }

    public override bool StartPrimaryAttack()
    {


        if (CheckFireRate())
        {
            m_LastFireTime = Time.time;
            Vector3 direction = transform.right;

            RaycastHit2D[] multiHit = Physics2D.CircleCastAll(m_FirePosition.position, m_Settings.HitCheckRadius, direction, m_Settings.AttackDistance, m_Settings.HitMask);
            
            //else
            //{
            //    hit = Physics2D.Raycast(m_FirePosition.position, direction, m_Settings.AttackDistance, m_Settings.HitMask);
            //}

            if (multiHit.Length > 0)
            {
                foreach(RaycastHit2D hit in multiHit)
                {
                    if (hit.collider.TryGetComponent(out IHittable hittable))
                    {
                        hittable.Hit(m_Settings.Damage, hit.point, hit.normal);
                    }

                    SpawnHitEffect(hit.point, hit.normal);
                }
            }

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


            if(m_Settings.FireEffect != null)
            {
                Instantiate(m_Settings.FireEffect, M_FireEffectPosition.position, M_FireEffectPosition.rotation);
            }

            //m_Player.Animation.PlaySwordAttack();
        }

        return false;
    }

    private void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        GameObject prefab = Instantiate(m_Settings.HitEffectPrefab);
        prefab.transform.position = position;
        prefab.transform.up = normal;
    }

    private bool CheckFireRate()
    {
        if (Time.time - m_LastFireTime > m_Settings.FireRate)
        {
            return true;
        }

        return false;
    }

    public override void CancelPrimaryAttack()
    {
        base.CancelPrimaryAttack();

        if (m_Settings.IsAudioOneShot)
        {
            return;
        }

        if (m_AudioSource.isPlaying)
        {
            if (Time.time - m_AudioPlayTime > m_Settings.MinimumAudioTime)
            {
                m_AudioSource.Stop();
            }
            else
            {
                if (m_AudioEndRoutine != null)
                {
                    StopCoroutine(m_AudioEndRoutine);
                }

                StartCoroutine(EndAudioAfterTime());
            }
        }
    }

    private IEnumerator EndAudioAfterTime()
    {
        while (Time.time - m_AudioPlayTime < m_Settings.MinimumAudioTime)
        {
            yield return null;
        }

        m_AudioSource.Stop();
    }
}
