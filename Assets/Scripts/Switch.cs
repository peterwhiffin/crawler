using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator m_Animator;
    private bool m_IsActivated;

    public void Interact()
    {
        m_IsActivated = !m_IsActivated;
        m_Animator.SetBool("Activated", m_IsActivated);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.Selector.InteractableInRange(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.Selector.InteractableOutOfRange(this);
        }
    }
}
