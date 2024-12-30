using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerAnimation : Animation
{
    [SerializeField] private Player m_Player;
    [SerializeField] private Slider m_HealthBar;
    [SerializeField] SpriteRenderer m_SpriteRenderer;
    [SerializeField] private float m_FlashDuration;
    [SerializeField] private Transform m_HealthbarPosition;
    [SerializeField, ColorUsage(true, true)] private Color m_FlashColor;
    [SerializeField, ColorUsage(true, true)] private Color m_DefaultColor;
    private void Start()
    {
        m_Player.Stats.Damaged += OnDamaged;
    }

    private void OnDestroy()
    {
        m_Player.Stats.Damaged -= OnDamaged;
    }

    private void Update()
    {
        m_HealthBar.transform.position = Camera.main.WorldToScreenPoint(m_HealthbarPosition.position);
    }

    public override void OnDamaged(float damage)
    {
        base.OnDamaged(damage);

        m_HealthBar.value = m_Player.Stats.CurrentHealth / m_Player.CrawlerSettings.MaxHealth;
        PlayHitAnimation();
    }

    private void PlayHitAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        float time = Time.time;
        m_SpriteRenderer.sharedMaterial.SetColor("_Color", m_FlashColor);

        while (Time.time - time < m_FlashDuration)
        {
            yield return null;
        }

        m_SpriteRenderer.sharedMaterial.SetColor("_Color", m_DefaultColor);
    }

    public void ResetPlayer()
    {
        m_HealthBar.value = 1f;
    }
}
