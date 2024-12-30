using UnityEngine;
using UnityEngine.InputSystem;
public class HotBar : MonoBehaviour
{


    private bool m_PrimaryAttackInput;
    [SerializeField] private Player m_Player;
    [SerializeField] private Equipment m_SelectedEquipment;

    public bool IsAttacking { get {  return m_PrimaryAttackInput; } }

    private void Start()
    {
        m_Player.Stats.Died += OnPlayerDied;
    }

    private void OnDestroy()
    {
        m_Player.Stats.Died -= OnPlayerDied;
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
        m_SelectedEquipment.StartPrimaryAttack();
    }

    private void PrimaryAttackCancelled()
    {
        m_SelectedEquipment.CancelPrimaryAttack();
    }
}
