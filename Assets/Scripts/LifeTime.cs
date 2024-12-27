using UnityEngine;

public class LifeTime : MonoBehaviour
{
    private float m_SpawnTime = 0f;
    [SerializeField] private float m_MaxLifeTime;

    private void Start()
    {
        m_SpawnTime = Time.time;
    }

    void Update()
    {
        if(Time.time - m_SpawnTime >= m_MaxLifeTime)
        {
            Destroy(gameObject);
        }
    }
}
