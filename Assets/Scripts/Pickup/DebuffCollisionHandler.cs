using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DebuffCollisionHandler : MonoBehaviour
{
    [SerializeField] private bool _deSpawnOnCollision = true;
    public float RespawnTimer = 4;

    [Header("Reset Velocity")]
    [SerializeField] private bool _resetVelocity = false;
    
    [Header("SlowDown")]
    [SerializeField] private bool _slowDown = false;
    [SerializeField] private float _slowDownTime = 1f;
    [SerializeField] [Range(0.01f, 0.95f)] private float _slowDownSpeedMultiplier = 0.5f;
    
    [Header("Push Backwards")]
    [SerializeField] private bool _pushBack = false;
    [SerializeField] private float _pushBackForce = 10f;
    
    [Header("Push Down")]
    [SerializeField] private bool _pushDown = false;
    [SerializeField] private float _pushDownForce = 10f;
    
    [Header("Slow Down Horizontal Controls")]
    [SerializeField] private bool _slowDownHorizontalControls = false;
    [SerializeField] private float _slowDownHorizontalControlsTime = 1f;
    [SerializeField] [Range(0.01f, 0.095f)] private float _slowDownHorizontalControlsMultiplier = 0.5f;
    
    [Header("Slow Down Vertical Controls")]
    [SerializeField] private bool _slowDownVerticalControls = false;
    [SerializeField] private float _slowDownVerticalControlsTime = 1f;
    [SerializeField] [Range(0.01f, 0.095f)] private float _slowDownVerticalControlsMultiplier = 0.5f;
    
    [Header("Reverse Controls")]
    [SerializeField] private bool _reverseControls = false;
    [SerializeField] private float _reverseControlsTime = 1f;
    
    [Header("Disable Controls")]
    [SerializeField] private bool _disableControls = false;
    [SerializeField] private float _disableControlsTime = 1f;
    
    [Header("Remove Shield Charge")]
    [SerializeField] private bool _removeShieldCharge = false;
    [SerializeField] private int _removeShieldChargeAmount = 1;
    
    [Header("Sounds")]
    [SerializeField] private AudioClip _soundToBePlayed;
    [SerializeField] [Range(0, 1)] private float _soundVolume = 1;

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
            var playerAudioSource = player.PlayerAudioSource;
            playerAudioSource.PlayOneShot(_soundToBePlayed, _soundVolume);

            if (player.ShieldSystem.IsShielded())
            {
                player.ShieldSystem.RemoveShieldCharge();
                return;
            }
            var buffHandler = player.BuffHandler;
            
            if (_resetVelocity)
            {
                buffHandler.ResetVelocity();
            }

            if (_slowDown)
            {
                buffHandler.SlowDown(_slowDownTime, _slowDownSpeedMultiplier);
            }
            
            if (_pushBack)
            {
                buffHandler.PushBackwards(_pushBackForce);
            }
            
            if (_pushDown)
            {
                buffHandler.PushDown(_pushDownForce);
            }
            
            if (_slowDownHorizontalControls)
            {
                buffHandler.SlowDownHorizontalControls(_slowDownHorizontalControlsTime, _slowDownHorizontalControlsMultiplier);
            }
            
            if (_slowDownVerticalControls)
            {
                buffHandler.SlowDownVerticalControls(_slowDownVerticalControlsTime, _slowDownVerticalControlsMultiplier);
            }
            
            if (_reverseControls)
            {
                buffHandler.ReverseControls(_reverseControlsTime);
            }
            
            if (_disableControls)
            {
                buffHandler.DisableControls(_disableControlsTime);
            }
            
            if (_removeShieldCharge)
            {
                buffHandler.RemoveShieldCharge(_removeShieldChargeAmount);
            }
            if(_deSpawnOnCollision)
                GetComponent<PoolOnImpact>().Pool();
        }
    }
}
