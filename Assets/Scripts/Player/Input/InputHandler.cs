using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputActions _input;

    public bool RightIsPressed { get; private set; }
    public bool LeftIsPressed { get; private set; }
    

    private void OnEnable()
    {
        _input = new InputActions();
        _input.Player.Enable();

        _input.Player.Right.started += OnRight;
        _input.Player.Right.canceled += OnRight;
        
        _input.Player.Left.started += OnLeft;
        _input.Player.Left.canceled += OnLeft;
        
        _input.Player.Pause.started += OnPause;
    }
    
    private void OnDisable()
    {
        _input.Player.Right.started -= OnRight;
        _input.Player.Right.canceled -= OnRight;
        
        _input.Player.Left.started -= OnLeft;
        _input.Player.Left.canceled -= OnLeft;
        
        _input.Player.Pause.started -= OnPause;
        
        _input.Player.Disable();
    }
    
    private void OnRight(InputAction.CallbackContext context)
    {
        RightIsPressed = context.started;
    }
    
    private void OnLeft(InputAction.CallbackContext context)
    {
        LeftIsPressed = context.started;
    }
    
    private void OnPause(InputAction.CallbackContext context)
    {
        GameManager.Instance.TogglePauseGame();
    }
}
