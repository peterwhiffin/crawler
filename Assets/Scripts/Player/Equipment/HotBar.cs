using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class HotBar : MonoBehaviour
{


    private bool m_PrimaryAttackInput;
    [SerializeField] private Player m_Player;
    [SerializeField] private Equipment m_SelectedEquipment;
    [SerializeField] private Grapple m_Grapple;
    [SerializeField] private List<Equipment> m_AllHotbarItems;
    private int m_CurrentHotbarIndex;   

    public bool IsAttacking { get {  return m_PrimaryAttackInput; } }

    private void Start()
    {
        m_Player.Stats.Died += OnPlayerDied;
        m_Grapple.GrappleHit += OnGrappleHit;
    }

    private void OnDestroy()
    {
        m_Player.Stats.Died -= OnPlayerDied;
        m_Grapple.GrappleHit -= OnGrappleHit;
    }

    public void OnScrollHotbarInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_CurrentHotbarIndex++;

            if(m_CurrentHotbarIndex >= m_AllHotbarItems.Count)
            {
                m_CurrentHotbarIndex = 0;
            }

            m_SelectedEquipment = m_AllHotbarItems[m_CurrentHotbarIndex];
        }
    }

    public void PrimaryAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_PrimaryAttackInput = true;
        }
        else if (context.canceled)
        {
            m_PrimaryAttackInput = false;
            PrimaryAttackCancelled();
        }
    }

    private void OnPlayerDied()
    {
        PrimaryAttackCancelled();
        m_PrimaryAttackInput = false;
    }

    private void Update()
    {
        if(m_PrimaryAttackInput)
        {
            if(m_Player.Stats.IsAlive)
                PrimaryAttackStarted();
        }
    }

    private void PrimaryAttackStarted()
    {
        m_PrimaryAttackInput = m_SelectedEquipment.StartPrimaryAttack();
    }

    private void PrimaryAttackCancelled()
    {
        m_SelectedEquipment.CancelPrimaryAttack();
    }

    private void OnGrappleHit(Vector2 position)
    {
        m_Player.Motor.HookGrapple(position);
    }
}
