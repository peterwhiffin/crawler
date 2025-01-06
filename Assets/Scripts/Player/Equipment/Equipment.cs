using UnityEngine;

public class Equipment : MonoBehaviour
{
    public virtual Transform FirePosition { get; }
    public virtual bool StartPrimaryAttack() { return true; }
    public virtual void CancelPrimaryAttack() { }
}
