using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    private float m_Length;
    [SerializeField] private List<Transform> m_AllBackgrounds;
    private Dictionary<Transform, Vector2> m_Backgrounds = new();
    [SerializeField] private Transform m_Camera;

    private void Start()
    {
        m_Length = m_AllBackgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;

        foreach (Transform t in m_AllBackgrounds)
        {
            m_Backgrounds.Add(t, t.position);
        }
    }

    private void Update()
    {
        foreach(var t in m_AllBackgrounds)
        {
            float temp = (m_Camera.position.x * (1 - t.position.z));
            float distance = (m_Camera.position.x * t.position.z);

            t.position = new Vector3(m_Backgrounds[t].x + distance, t.position.y, t.position.z);

            if (temp > m_Backgrounds[t].x + m_Length)
            {
                Vector2 current = m_Backgrounds[t];
                current.x += m_Length * 2;
                m_Backgrounds[t] = current;
            }
            else if (temp < m_Backgrounds[t].x - m_Length)
            {
                Vector2 current = m_Backgrounds[t];
                current.x -= m_Length * 2;
                m_Backgrounds[t] = current;
            }
        }
    }
}
