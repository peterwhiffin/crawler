using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] protected Animator m_Animator;
    public virtual void OnDamaged(float damage) { }
    public virtual void OnHealed() { }
}
