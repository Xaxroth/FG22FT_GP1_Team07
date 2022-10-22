using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    
    private InputHandler _input;
    public Rigidbody Rigidbody { get; private set; }
    private Animator _playerAnimator;
    private UIManager _uiManager;

    [Header("Input")]
    private Vector3 _moveVector;

    private float _leftInput;
    private float _rightInput;
    [SerializeField] private bool _controlsDisabled;
    public Vector3 Velocity { get; private set; }
    public bool ControlsDisabled
    {
        set => _controlsDisabled = value;
    }
    
    [Header("Movement Horizontal")]
    [SerializeField] private float _minHorizontalMoveSpeed = 0.5f;
    [SerializeField] private float _maxHorizontalMoveSpeed = 60.0f;
    [SerializeField] private float _moveHorizontalSpeed = 30.0f;
    public float MoveHorizontalSpeed
    {
        get => _moveHorizontalSpeed;
        set => _moveHorizontalSpeed = Mathf.Clamp(value, _minHorizontalMoveSpeed, _maxHorizontalMoveSpeed);
    }
    [Header("Movement Vertical")]
    [SerializeField] private float _minVerticalMoveSpeed = 0.5f;
    [SerializeField] private float _maxVerticalMoveSpeed = 60.0f;
    [SerializeField] private float _moveVerticalSpeed = 30.0f;
    public float MoveVerticalSpeed
    
    {
        get => _moveVerticalSpeed;
        set => _moveVerticalSpeed = Mathf.Clamp(value, _minVerticalMoveSpeed, _maxVerticalMoveSpeed);
    }
    [Header("Forward Movement")]
    [SerializeField] private float _minForwardMoveSpeed = 0.1f;
    [SerializeField] private float _maxForwardMoveSpeed = 10.0f;
    [SerializeField] private float _moveForwardSpeed = 1.0f;
    public float MoveForwardSpeedMultiplier
    {
        get => _moveForwardSpeed;
        set => _moveForwardSpeed = Mathf.Clamp(value, _minForwardMoveSpeed, _maxForwardMoveSpeed);
    }
    [Header("Movement Total Max Speed")]
    [SerializeField] private float _maxSpeed = 10.0f;
    public float MaxSpeed
    {
        get => _maxSpeed;
        set => _maxSpeed = value;
    }

    public bool ReverseControls {get; set;}
    
    [Header("Gravity")] 
    [SerializeField] private float _gravityFallCurrent = 0.0f;
    [SerializeField] private float _gravityFallMin = 0.0f;
    [SerializeField] private float _gravityFallMaxMultiplier = 0.5f;
    [SerializeField] private float _fallTimer = 0.0f;
    [SerializeField] private float _gravityFallIncrement = 0.1f;
    private bool _gravityReset = false;
    
    [Header("Boundaries")]
    [SerializeField] private float _bufferStartDistance = 10.0f;
    [SerializeField] private float _minDistanceToBoundary = 0.5f;
    [SerializeField] private LayerMask _boundaryLayerMask;
    

    void Awake()
    {
        _input = GetComponent<InputHandler>();
        _playerAnimator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _uiManager = GameManager.Instance.UIManager;
    }

    void FixedUpdate()
    {
        SetMoveInput();
        _moveVector = GetHorizontalForce();
        if(_controlsDisabled) _moveVector = Vector3.zero;
        _gravityReset = ResetGravity();
        _moveVector.x = ReverseControls ? -_moveVector.x : _moveVector.x;
        _moveVector.x *= _moveHorizontalSpeed;

        _moveVector.y += GetUpForce(); 
        _moveVector.y *= _moveVerticalSpeed;
        _moveVector.y += GetGravityForce();
        
        _moveVector.z = GetForwardForce();
        _moveVector.z *= _moveForwardSpeed;
        
        Rigidbody.AddRelativeForce(_moveVector);
        Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, _maxSpeed);
        Velocity = Rigidbody.velocity;
        
        EnforceBoundaries();

        SetAnimation();
    }

    private void SetAnimation()
    {
        if (_leftInput >= 1)
        {
            _playerAnimator.SetBool("TurningRight", true);
        }
        else
        {
            _playerAnimator.SetBool("TurningRight", false);
        }

        if (_rightInput >= 1)
        {
            _playerAnimator.SetBool("TurningLeft", true);
        }
        else
        {
            _playerAnimator.SetBool("TurningLeft", false);
        }

        if (_leftInput >= 1 && _rightInput >= 1)
        {
            _playerAnimator.SetBool("TurningUp", true);
        }
        else
        {
            _playerAnimator.SetBool("TurningUp", false);
        }
    }

    private bool ResetGravity()
    {
        return _input.RightIsPressed || _input.LeftIsPressed;
    }

    private void SetMoveInput()
    {
        _rightInput = _input.RightIsPressed ? 1 : 0;
        _leftInput = _input.LeftIsPressed ? 1 : 0;
    }

    private Vector3 GetHorizontalForce()
    {
        return new Vector3(_leftInput - _rightInput, 0, 0);
    }
    
    private float GetUpForce()
    {
        return (_rightInput + _leftInput) / 2.0f;
    }

    private float GetForwardForce()
    {
        return (_rightInput + _leftInput) / 2.0f;
    }
    
    private float GetGravityForce()
    {
        var gravity = _moveVector.y;

        if (_gravityReset)
        {
            _gravityFallCurrent = _gravityFallMin;
        }
        else
        {
            _fallTimer -= Time.fixedDeltaTime;
            if (_fallTimer < 0.0f)
            {
                float gravityFallMax = _moveVerticalSpeed * _gravityFallMaxMultiplier;
                float increment = (gravityFallMax - _gravityFallMin) * _gravityFallIncrement;
                if (_gravityFallCurrent < gravityFallMax)
                    _gravityFallCurrent += increment;
                _fallTimer = _gravityFallIncrement;
            }
            gravity = -_gravityFallCurrent;
        }
        return gravity;
    }

    private void EnforceBoundaries()
    {                
        //x = right .6 -> -75
        //y = top .7 -> .8
        //z = left -.2 -> -.1
        //w = bottom -.23 -> -.13
        var boundsWarningVector = new Vector4(.55f, .6f, 0, -.03f);
        //bottom
        if(Physics.Raycast(transform.position, Vector3.down, out var hit, Mathf.Infinity, _boundaryLayerMask))
        {
            var distance = hit.distance; 
            if (distance < _bufferStartDistance)
            {
                float diffLerp = Mathf.InverseLerp(0, _bufferStartDistance, distance);
                if(Rigidbody.velocity.y < 0)
                {
                    Vector3 newVel = Rigidbody.velocity;

                    newVel.y *= diffLerp;
                    if (distance < _minDistanceToBoundary)
                    {
                        newVel.y = 0;
                    }

                    Rigidbody.velocity = newVel;
                }
                var value = Mathf.Lerp(-.43f ,-.13f,diffLerp);
                boundsWarningVector.w = value;
            }
        }
        //top
        if(Physics.Raycast(transform.position, Vector3.up, out var hit2, Mathf.Infinity, _boundaryLayerMask))
        {
            var distance = hit2.distance; 
            if (distance < _bufferStartDistance)
            {
                float diffLerp = Mathf.InverseLerp(0, _bufferStartDistance, distance);
                if(Rigidbody.velocity.y > 0)
                {
                    Vector3 newVel = Rigidbody.velocity;

                    newVel.y *= diffLerp;
                    if (distance < _minDistanceToBoundary)
                    {
                        newVel.y = 0;
                    }

                    Rigidbody.velocity = newVel;
                }
                var value =  Mathf.Lerp(1f, .7f,diffLerp);
                boundsWarningVector.y = value;
            }
        }
        //left
        if(Physics.Raycast(transform.position, Vector3.left, out var hit3, Mathf.Infinity, _boundaryLayerMask))
        {
            var distance = hit3.distance; 
            if (distance < _bufferStartDistance)
            {
                float diffLerp = Mathf.InverseLerp(0, _bufferStartDistance, distance);
                if(Rigidbody.velocity.x < 0)
                {
                    Vector3 newVel = Rigidbody.velocity;

                    newVel.x *= diffLerp;
                    if (distance < _minDistanceToBoundary)
                    {
                        newVel.x = 0;
                    }

                    Rigidbody.velocity = newVel;
                }
                var value = Mathf.Lerp(-.4f, -.1f,diffLerp);
                boundsWarningVector.z = value;
            }
        }   
        //right
        if(Physics.Raycast(transform.position, Vector3.right, out var hit4, Mathf.Infinity, _boundaryLayerMask))
        {
            var distance = hit4.distance; 
            if (distance < _bufferStartDistance)
            {
                float diffLerp = Mathf.InverseLerp(0, _bufferStartDistance, distance);
                if(Rigidbody.velocity.x > 0)
                {
                    Vector3 newVel = Rigidbody.velocity;

                    newVel.x *= diffLerp;
                    if (distance < _minDistanceToBoundary)
                    {
                        newVel.x = 0;
                    }

                    Rigidbody.velocity = newVel;
                }
                var value = Mathf.Lerp(.95f, .6f,diffLerp);
                boundsWarningVector.x = value;
            }
        }   
        
        _uiManager.SetBoundaryEffect(boundsWarningVector);
    }

    /// <summary>
    /// Resets the player's velocity to Vector3.zero
    /// </summary>
    public void ResetVelocity()
    {
        Rigidbody.velocity = Vector3.zero;
    }
    
    /// <summary>
    /// Pushes the player in the direction of the given vector
    /// ForceMode defaults to ForceMode.Impulse
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    /// <param name="forceMode"></param>
    public void Push(Vector3 direction, float force, ForceMode forceMode = ForceMode.Impulse)
    {
        Rigidbody.AddForce(direction * force, forceMode);
    }


}
