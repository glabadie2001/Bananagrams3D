using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SerializedMonoBehaviour, InputActions.IPlayerActions
{
    public static InputManager Inst;
    
    [Header("Debug")]
    public Vector2 mousePos;
    
    InputActions actions;

    public event Action<InputAction.CallbackContext> OnLookEvent;
    public event Action<InputAction.CallbackContext> OnSelectStartEvent;
    public event Action<InputAction.CallbackContext> OnSelectEndEvent;
    
    private void Awake()
    {
        //Singleton boilerplate
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(this);

        if (actions == null)
        {
            actions = new InputActions();
            actions.Player.SetCallbacks(this);
        }

        actions.Player.Enable();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
        OnLookEvent?.Invoke(context);
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.started)
            OnSelectStartEvent?.Invoke(context);
        else if (context.canceled)
            OnSelectEndEvent?.Invoke(context);
    }
}
