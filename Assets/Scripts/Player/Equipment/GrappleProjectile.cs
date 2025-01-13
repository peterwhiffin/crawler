using UnityEngine;

public class GrappleProjectile : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_MaxLifeTime;
    [SerializeField] private GameObject m_HitPrefab;
    [SerializeField] private LayerMask m_HitMask;
    private bool m_hasFired = false;
    private float m_LifeTime = 0f;
    private Vector3 m_PreviousPosition;
    private float m_Damage = 0f;
    private float m_Radius = 0f;
    [SerializeField] private float m_MaxDistance;
    [SerializeField] private Grapple m_Grapple;
    private float TravelDistance = 0f;

    public void Fire(Vector3 position, Quaternion rotation, float speed, float damage, float radius = 0f)
    {
        transform.position = position;
        transform.rotation = rotation;
        m_PreviousPosition = position;
        m_Speed = speed;
        m_hasFired = true;
        m_Damage = damage;
        m_Radius = radius;
        TravelDistance = 0f;
    }

    private void Update()
    {
        if (!m_hasFired)
            return;

        CheckHit();

        

        Move();
    }

    private void CheckHit()
    {
        Vector3 direction = transform.position - m_PreviousPosition;
        RaycastHit2D hit;

        if (m_Radius != 0f)
        {
            hit = Physics2D.CircleCast(m_PreviousPosition, m_Radius, direction, direction.magnitude, m_HitMask);
        }
        else
        {
            hit = Physics2D.Raycast(m_PreviousPosition, direction, direction.magnitude, m_HitMask);
        }

        if (hit)
        {
            bool canReelObjectIn = false;
            m_hasFired = false;

            if (hit.collider.TryGetComponent(out IGrappleable grappleable))
            {
                canReelObjectIn = grappleable.GrappleHit(m_Damage, hit.point, hit.normal, transform);

                if (canReelObjectIn)
                {
                    m_Grapple.ReelObjectIn(hit.point, grappleable);
                }
                else
                {
                    m_Grapple.ReelPlayerIn(hit.point);
                }
            }
            else
            {
                m_Grapple.ReelPlayerIn(hit.point);
            }

            SpawnHitEffect(hit.point, hit.normal);
            //Destroy(gameObject);
        }
    }

    private void Move()
    {


        m_PreviousPosition = transform.position;
        transform.position += transform.right * m_Speed * Time.deltaTime;

        if(TravelDistance >= m_MaxDistance)
        {
            m_hasFired = false;
            m_Grapple.ProjectileMissed();
        }

        TravelDistance += m_Speed * Time.deltaTime;
        //m_LifeTime += Time.deltaTime;
    }

    private void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        GameObject prefab = Instantiate(m_HitPrefab);
        prefab.transform.position = position;
        prefab.transform.up = normal;
    }
}
