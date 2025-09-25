using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Inst;

    public InputFrame lastInput;

    [SerializeField] bool clickHeld = false;

    public string[] actions = {
        "Click"
     };
    Dictionary<string, InputAction> actionMap = new Dictionary<string, InputAction>();

    private void Awake()
    {
        //Singleton boilerplate
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(this);

        //Initialize mapping to global actions (see ProjectSettings/Input in engine)
        foreach (var action in actions)
        {
            actionMap.Add(action, InputSystem.actions.FindAction(action));
        }
    }

    private void Start()
    {
        lastInput = ProcessInput();
    }

    private void Update()
    {
        lastInput = ProcessInput();
    }

    public InputFrame ProcessInput()
    {
        Vector2 mousePos = actionMap["Look"].ReadValue<Vector2>();

        bool clickDown = actionMap["Click"].WasPressedThisFrame();
        bool clickUp = actionMap["Click"].WasReleasedThisFrame();
        clickHeld = (clickHeld || clickDown) && !clickUp;

        return new InputFrame(mousePos, clickDown, clickUp, clickHeld);
    }
}

[System.Serializable]
public struct InputFrame
{
    public Vector2 mousePos;
    public bool clickDown;
    public bool clickUp;
    public bool clickHeld;

    public InputFrame(Vector2 _mousePos, bool _clickDown, bool _clickUp, bool _clickHeld)
    {
        mousePos = _mousePos;
        clickDown = _clickDown;
        clickUp = _clickUp;
        clickHeld = _clickHeld;
    }
}
