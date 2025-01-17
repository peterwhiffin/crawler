using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAnimation : AnimationController
{
    [SerializeField] private GameObject HealthbarPrefab;
    [SerializeField] private Enemy m_Enemy;
    //[SerializeField] private Slider m_HealthBar;
    [SerializeField] SpriteRenderer m_SpriteRenderer;
    [SerializeField] private float m_FlashDuration;
    [SerializeField] private Transform m_HealthbarPosition;
    [SerializeField, ColorUsage(true, true)] private Color m_FlashColor;
    [SerializeField, ColorUsage(true, true)] private Color m_DefaultColor;

    public Action OnUpdate = delegate { };

    private void OnDestroy()
    {
        m_Enemy.Stats.Damaged -= OnDamaged;
        OnUpdate -= On_Update;
        //Destroy(m_HealthBar.gameObject);
    }

    public void Initialize()
    {

        m_Enemy.Stats.Damaged += OnDamaged;
        OnUpdate += On_Update;
    }

    private void Update()
    {
        OnUpdate.Invoke();
    }

    public void UpdateMove(bool move)
    {
        m_Animator.SetBool("Move", move);
    }

    private void On_Update()
    {
        //m_HealthBar.transform.position = Camera.main.WorldToScreenPoint(m_HealthbarPosition.position);
    }

    public override void OnDamaged(float damage)
    {
        base.OnDamaged(damage);

        //m_HealthBar.value = m_Enemy.Stats.CurrentHealth / m_Enemy.Settings.MaxHealth;

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
        m_SpriteRenderer.material.SetFloat("_Flash", 1f);

        while(Time.time - time < m_FlashDuration)
        {
            yield return null;
        }

        m_SpriteRenderer.material.SetFloat("_Flash", 0f);
    }
}
