using UnityEngine;

public class Equipment : MonoBehaviour
{
    public virtual Transform FirePosition { get; }
    public virtual void StartPrimaryAttack() { }
    public virtual void CancelPrimaryAttack() { }
}
