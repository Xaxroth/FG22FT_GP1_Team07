using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuffCollisionHandler : MonoBehaviour
{
    [SerializeField] private bool _deSpawnOnCollision = true;
    public float RespawnTimer = 4;
    
    [Header("Add Fuel")]
    [SerializeField] private bool _addFuel = false;
    [SerializeField] private float _fuelAmount = 0.0f;

    [Header("SpeedUp")]
    [SerializeField] private bool _speedUp = false;
    [SerializeField] private bool _instantEffect= false;
    [SerializeField] private float _speedUpTime = 0.0f;
    [SerializeField] private float _speedUpAmountMultiplier = 0.0f;
    
    [Header("Shield")]
    [SerializeField] private bool _addShield = false;
    [SerializeField] private int _shieldCharges = 0;
    
    [Header("Add Time")]
    [SerializeField] private bool _addTime = false;
    [SerializeField] private float _timeAmount = 0.0f;

    [Header("Audio")]
    [SerializeField] private AudioSource _playerAudioSource;
    [SerializeField] private AudioClip _soundToBePlayed;
    [SerializeField] [Range(0, 1)] private float _soundVolume;

    private void Awake()
    {
        if (_deSpawnOnCollision)
        {
            var pool = gameObject.AddComponent<PoolOnImpact>();
            pool.Init(RespawnTimer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerState player))
        {
            _playerAudioSource = player.PlayerAudioSource;
            var buffHandler = player.BuffHandler;

            if (_addFuel)
            {
                _playerAudioSource.PlayOneShot(_soundToBePlayed, _soundVolume);
                buffHandler.AddFuel(_fuelAmount);
            }

            if (_speedUp)
            {
                _playerAudioSource.PlayOneShot(_soundToBePlayed, _soundVolume);
                buffHandler.SpeedUp(_speedUpTime, _speedUpAmountMultiplier, _instantEffect);
            }

            if (_addShield)
            {
                _playerAudioSource.PlayOneShot(_soundToBePlayed, 0.7f);
                buffHandler.AddShieldCharge(_shieldCharges);
            }

            if (_addTime)
            {
                GameManager.Instance.TimerSystem.AddTime(_timeAmount);
            }
            
            if(_deSpawnOnCollision)
                GetComponent<PoolOnImpact>().Pool();
        }
    }
}
