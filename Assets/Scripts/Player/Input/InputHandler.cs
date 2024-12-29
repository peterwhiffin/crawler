using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; set; }
    public Vector2 LookInput { get; set; }
    public bool FireInput { get; set; }
    public bool StretchInput { get; set; }


    public void OnMoveInput(InputAction.CallbackContext context) 
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            FireInput = true;
        }
        else if (context.canceled)
        {
            FireInput = false;
        }        
    }

    public void OnStretchInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StretchInput = true;
        }
        else if (context.canceled)
        {
            StretchInput = false;
        }
    }
}
