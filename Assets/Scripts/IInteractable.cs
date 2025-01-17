using UnityEngine;

public interface IInteractable
{
    public void Interact() { }
    public void UpdateTriggerView(bool interactorInRange) { }
}
