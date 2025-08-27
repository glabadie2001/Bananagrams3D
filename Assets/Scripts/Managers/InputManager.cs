using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string clickActionName = "Attack";
    [SerializeField] private string pointerActionName = "Look";
    
    private InputAction clickAction;
    private InputAction pointerAction;
    
    public event Action OnClickStarted;
    public event Action OnClickCanceled;
    public event Action<Vector2> OnPointerMoved;
    
    public bool IsClicking { get; private set; }
    public Vector2 PointerPosition => GetPointerPosition();
    
    private void Awake()
    {
        SetupInputActions();
    }
    
    private void SetupInputActions()
    {
        if (inputActions != null)
        {
            var actionMap = inputActions.FindActionMap(actionMapName);
            if (actionMap != null)
            {
                clickAction = actionMap.FindAction(clickActionName);
                pointerAction = actionMap.FindAction(pointerActionName);
            }
        }
        
        if (clickAction == null)
        {
            Debug.LogWarning("Click action not found, falling back to mouse input");
        }
        
        if (pointerAction == null)
        {
            Debug.LogWarning("Pointer action not found, falling back to mouse input");
        }
    }
    
    private void OnEnable()
    {
        if (clickAction != null)
        {
            clickAction.performed += OnClickPerformed;
            clickAction.canceled += OnClickCanceledInternal;
            clickAction.Enable();
        }
        
        if (pointerAction != null)
        {
            pointerAction.performed += OnPointerPerformed;
            pointerAction.Enable();
        }
    }
    
    private void OnDisable()
    {
        if (clickAction != null)
        {
            clickAction.performed -= OnClickPerformed;
            clickAction.canceled -= OnClickCanceledInternal;
            clickAction.Disable();
        }
        
        if (pointerAction != null)
        {
            pointerAction.performed -= OnPointerPerformed;
            pointerAction.Disable();
        }
    }
    
    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        IsClicking = true;
        OnClickStarted?.Invoke();
    }
    
    private void OnClickCanceledInternal(InputAction.CallbackContext context)
    {
        IsClicking = false;
        OnClickCanceled?.Invoke();
    }
    
    private void OnPointerPerformed(InputAction.CallbackContext context)
    {
        Vector2 pointerPos = context.ReadValue<Vector2>();
        OnPointerMoved?.Invoke(pointerPos);
    }
    
    private Vector2 GetPointerPosition()
    {
        if (pointerAction != null)
        {
            return Mouse.current.position.ReadValue();
        }
        return Input.mousePosition;
    }
    
    public Ray GetCameraRay(Camera camera)
    {
        return camera.ScreenPointToRay(PointerPosition);
    }
}