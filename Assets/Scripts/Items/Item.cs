using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour, IPickup
{
    [field: SerializeField] public ItemDetails Details { get; private set; }
    private Coroutine MoveRoutine;
    [SerializeField] private Animator m_Animator;

    public void PlayerInRange(Player player)
    {
        if(MoveRoutine == null)
        {
           MoveRoutine = StartCoroutine(MoveToPlayer(player));
        }
    }

    private IEnumerator MoveToPlayer(Player player)
    {
        float timeElapsed = 0f;

        while(Vector3.Distance(transform.position, player.transform.position) > .2f)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, timeElapsed / .3f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        player.ItemPickedUp(this);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(m_Animator != null)
        {
            m_Animator.SetTrigger("Splash");
        }
    }
}
