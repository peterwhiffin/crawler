using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    private IInteractable m_CurrentInteractable;


    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Interact();
        }
    }

    public void InteractableInRange(IInteractable interactable)
    {
        m_CurrentInteractable = interactable;
    }

    public void InteractableOutOfRange(IInteractable interactable)
    {
        if(m_CurrentInteractable != interactable)
        {
            return;
        }

        m_CurrentInteractable = null;
    }

    private void Interact()
    {
        if(m_CurrentInteractable == null)
        {
            return;
        }

        m_CurrentInteractable.Interact();
    }
}
