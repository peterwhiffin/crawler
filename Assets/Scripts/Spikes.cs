using UnityEngine;

public class Spikes : MonoBehaviour, IDamaging
{
    [SerializeField] private float m_TimePerHit;
    private float m_LastHit;
    [SerializeField] private float m_Damage;

    private void Start()
    {
        m_LastHit = Time.time;
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out IHittable hittable))
        {
            if (Time.time - m_LastHit > m_TimePerHit)
            {
                m_LastHit = Time.time;

                hittable.Hit(m_Damage);
            }
        }
    }
}
