using UnityEngine;

public class ContactDamager : MonoBehaviour
{
    [SerializeField] private float m_Damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.TryGetComponent(out IHittable hittable))
        {
            hittable.Hit(m_Damage, collision.contacts[0].point, collision.contacts[0].normal);
        }
    }
}

