using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
    [SerializeField] private Player m_Player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IPickup item))
        {
            item.PlayerInRange(m_Player);
        }
    }
}
