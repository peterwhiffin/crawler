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
    [SerializeField, ColorUsage(true, true)] private Color m_DefaultRopeColor;
    [SerializeField] private Material m_RopeMaterial;
    private Coroutine m_FlashRoutine;

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
        if(m_FlashRoutine != null)
        {
            StopCoroutine(m_FlashRoutine);
        }

       m_FlashRoutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        float time = Time.time;
        m_SpriteRenderer.sharedMaterial.SetColor("_Color", m_FlashColor);
        m_RopeMaterial.SetColor("_Color", m_FlashColor);

        while (Time.time - time < m_FlashDuration)
        {
            var t = (Time.time - time) / m_FlashDuration;
            var currentColor = m_RopeMaterial.GetColor("_Color");
            var nextColor = Color.Lerp(currentColor, m_DefaultRopeColor, t);
            m_RopeMaterial.SetColor("_Color", nextColor);

            currentColor = m_SpriteRenderer.sharedMaterial.GetColor("_Color");
            nextColor = Color.Lerp(currentColor, m_DefaultColor, t);
            m_SpriteRenderer.sharedMaterial.SetColor("_Color", nextColor);
            

            yield return null;
        }

        m_SpriteRenderer.sharedMaterial.SetColor("_Color", m_DefaultColor);
        m_RopeMaterial.SetColor("_Color", m_DefaultRopeColor);
    }



    public void ResetPlayer()
    {
        m_HealthBar.value = 1f;
    }
}
