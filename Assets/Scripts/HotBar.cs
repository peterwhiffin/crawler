using UnityEngine;
using UnityEngine.InputSystem;
public class HotBar : MonoBehaviour
{


    private bool m_PrimaryAttackInput;

    [SerializeField] private Equipment m_SelectedEquipment;

    public void PrimaryAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_PrimaryAttackInput = true;
        }
        else if (context.canceled)
        {
            m_PrimaryAttackInput = false;
        }
    }

    private void Update()
    {
        if(m_PrimaryAttackInput)
        {
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
