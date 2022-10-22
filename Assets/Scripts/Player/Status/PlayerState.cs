using System;
using System.Collections;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public FuelSystem FuelSystem { get; private set; }
    public BuffHandler BuffHandler { get; private set; }
    public ShieldSystem ShieldSystem { get; private set; }
    public AudioSource PlayerAudioSource { get; private set; }
    
    private PlayerMovement _playerMovement;
    private InputHandler _inputHandler;
    private CameraController _cameraController;
    
    [Header("Fuel")]
    [SerializeField] private float _maxFuel;
    [SerializeField] private float _startFuel;
    [SerializeField] private float _fuelConsumptionMultiplier = 1f;
    [SerializeField] private ParticleSystem _speedUpParticles;

    [Header("Thruster Particle Systems")]
    [SerializeField] private ParticleSystem _playerLeftParticles;
    [SerializeField] private ParticleSystem _playerRightParticles;
    private bool _canPlayOneShot = true;

    [Header("Thruster Audio Clips")]
    [SerializeField] private AudioClip _playerLeftThrusterClip;
    [SerializeField] private AudioClip _playerRightThrusterClip;

    [Header("Shield")]
    [SerializeField] private int _maxShieldCharges;
    [SerializeField] private float _shieldDurationMax;
    
    

    private void Awake()
    {
        FuelSystem = new FuelSystem(_maxFuel, _startFuel, _fuelConsumptionMultiplier);
        
        _playerMovement = GetComponent<PlayerMovement>();
        BuffHandler = gameObject.AddComponent<BuffHandler>();
        BuffHandler.Init(_playerMovement, this);
        _inputHandler = GetComponent<InputHandler>();
        
        ShieldSystem = new ShieldSystem(_maxShieldCharges, _shieldDurationMax);
        _speedUpParticles.Stop();
        
        PlayerAudioSource = GetComponent<AudioSource>();
        
        FuelSystem.OnFuelFull += OnFuelFull;
        
    }

    private void Start()
    {
        _cameraController = GameManager.Instance.MainCamera.GetComponent<CameraController>();
    }

    private void OnDisable()
    {
        FuelSystem.OnFuelFull -= OnFuelFull;
    }

    private void FixedUpdate()
    {

        if (ShieldSystem.IsShielded())
        {
            ShieldSystem.RemoveShieldDuration(Time.fixedDeltaTime);
        }

        ToggleRightThruster(_inputHandler.RightIsPressed);
        ToggleLeftThruster(_inputHandler.LeftIsPressed);

    }

    private void ToggleRightThruster(bool toggle)
    {
        if(toggle)
        {
            if (_canPlayOneShot && _playerRightParticles.isStopped)
                StartCoroutine(PlaySoundOneShot(_playerRightThrusterClip));
            PlayerAudioSource.Play();
            _playerRightParticles.Play();
        }
        else
        {
            PlayerAudioSource.Stop();
            _playerRightParticles.Stop();
        }
    }
    
    private void ToggleLeftThruster(bool toggle)
    {
        if(toggle)
        {
            if (_canPlayOneShot && _playerLeftParticles.isStopped)
                StartCoroutine(PlaySoundOneShot(_playerLeftThrusterClip));
            PlayerAudioSource.Play();
            _playerLeftParticles.Play();
        }
        else
        {
            PlayerAudioSource.Stop();
            _playerLeftParticles.Stop();
        }
    }

    private IEnumerator PlaySoundOneShot(AudioClip clip)
    {
        PlayerAudioSource.PlayOneShot(clip);
        _canPlayOneShot = false;
        yield return new WaitForSeconds(clip.length);
        _canPlayOneShot = true;
    }

    private void OnFuelFull()
    {
        StartCoroutine(UseUltimate());
    }

    private IEnumerator UseUltimate()
    {
        var maxSpeed = _playerMovement.MaxSpeed;
        var horizontalSpeed = _playerMovement.MoveHorizontalSpeed;
        var forwardSpeed = _playerMovement.MoveForwardSpeedMultiplier;
        _speedUpParticles.Play();
        
        while(FuelSystem.Fuel > 0)
        {
            FuelSystem.ConsumeFuel(Time.deltaTime);
            _playerMovement.MaxSpeed = maxSpeed * 2;
            _playerMovement.MoveHorizontalSpeed = horizontalSpeed * 2;
            _playerMovement.MoveForwardSpeedMultiplier = forwardSpeed * 2;
            _cameraController.AddOffset(Time.deltaTime);
            yield return null;
        }
        _speedUpParticles.Stop();
        _cameraController.RevertOffsetToNormal();
        _playerMovement.MaxSpeed = maxSpeed;
        _playerMovement.MoveHorizontalSpeed = horizontalSpeed;
        _playerMovement.MoveForwardSpeedMultiplier = forwardSpeed;
    }
}
