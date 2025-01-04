using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerAnimation : Animation
{
    [SerializeField] private Player m_Player;
    [SerializeField] private Transform m_Graphic;
    [SerializeField] private Slider m_HealthBar;
    [SerializeField] SpriteRenderer m_SpriteRenderer;
    [SerializeField] private float m_FlashDuration;
    [SerializeField] private Transform m_HealthbarPosition;
    [SerializeField, ColorUsage(true, true)] private Color m_FlashColor;
    [SerializeField, ColorUsage(true, true)] private Color m_DefaultColor;
    [SerializeField, ColorUsage(true, true)] private Color m_DefaultRopeColor;
    [SerializeField, ColorUsage(true, true)] private Color m_HealColor;
    [SerializeField] private Material m_RopeMaterial;

    private Coroutine m_FlashRoutine;

    private void Start()
    {
        m_Player.Stats.Damaged += OnDamaged;
        m_Player.Stats.Healed += OnHealed;
    }

    private void OnDestroy()
    {
        m_Player.Stats.Damaged -= OnDamaged;
        m_Player.Stats.Healed -= OnHealed;
    }

    private void Update()
    {
        m_HealthBar.transform.position = Camera.main.WorldToScreenPoint(m_HealthbarPosition.position);
    }

    public override void OnDamaged(float damage)
    {
        base.OnDamaged(damage);

        m_HealthBar.value = m_Player.Stats.CurrentHealth / m_Player.CrawlerSettings.MaxHealth;
        PlayFlashAnimation(m_FlashColor, m_FlashDuration);
    }

    public override void OnHealed()
    {
        base.OnHealed();
        m_HealthBar.value = m_Player.Stats.CurrentHealth / m_Player.CrawlerSettings.MaxHealth;
        PlayFlashAnimation(m_HealColor, m_FlashDuration + .3f);
    }

    private void PlayFlashAnimation(Color flashColor, float duration)
    {
        if(m_FlashRoutine != null)
        {
            StopCoroutine(m_FlashRoutine);
        }

       m_FlashRoutine = StartCoroutine(HitFlash(flashColor, duration));
    }

    private IEnumerator HitFlash(Color flashColor, float duration)
    {
        float time = Time.time;
        m_SpriteRenderer.sharedMaterial.SetColor("_Color", flashColor);
        m_RopeMaterial.SetColor("_Color", flashColor);

        while (Time.time - time < duration)
        {
            var t = (Time.time - time) / duration;
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

    public void SetCapsulePosition(Vector2 moveDirection)
    {    
        m_Graphic.localPosition = Vector3.MoveTowards(m_Graphic.localPosition, moveDirection.normalized * m_Player.CrawlerSettings.MaxGraphicLocalStretch, Time.deltaTime * m_Player.CrawlerSettings.LocalStretchLerp);
    }

    public void ResetPlayer()
    {
        m_HealthBar.value = 1f;
    }
}
