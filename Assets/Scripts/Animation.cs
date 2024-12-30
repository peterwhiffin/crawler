using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Animation : MonoBehaviour
{
    [SerializeField] protected Animator m_Animator;
    public virtual void OnDamaged(float damage) { }
}
