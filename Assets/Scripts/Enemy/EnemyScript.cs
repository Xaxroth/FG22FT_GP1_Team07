using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Logistics")]
    [SerializeField] private MasterSpawner _masterSpawnerScript;
    [SerializeField] private SpawnHazard _hazardSpawnerScript;
    [SerializeField] private EnemyScript _enemy;
    [SerializeField] private Transform _player;

    [SerializeField] private ParticleSystem _bigSmokeParticles;
    [SerializeField] private ParticleSystem _smallSmokeParticles;

    [SerializeField] private PlayerMovement _playerMovement;

    [SerializeField] private ParticleSystem _powerUpParticles;

    [SerializeField] private Vector3 _playerVelocity;

    [SerializeField] private string _objectString;
    [SerializeField] private int _spawnIndex;

    [SerializeField] private int _minValue;
    [SerializeField] private int _maxValue;

    [SerializeField] private bool moving;

    [Header("Custom Controls")]
    [SerializeField] public float DistanceToPlayer = 120f;
    [SerializeField] public float MaxDistanceToPlayer = 140f;

    [SerializeField] public float currentDistanceToNextChunk = 750f;
    [SerializeField] public float maxDistanceToNextChunk = 750f;

    [Header("Random Move Pattern")]

    [SerializeField] private float _minTravelDistance = -10;
    [SerializeField] private float _maxTravelDistance = 10;
    [SerializeField] private float _attackCooldown = 1.5f;
    [SerializeField] private float _enemyMovementSpeed = 1f;

    [Header("Punishment and Difficulty")]

    [SerializeField] private float _velocityCutoff = 20;
    [SerializeField] private float _passiveDistanceGainFromPlayer = 2;
    [SerializeField] private float _punishmentMoveSpeed = 10;
    [SerializeField] private float _punishmentMaxDistance = 100;
    [SerializeField] private float _distanceMultiplier = 1f;


    [Header("Vectors & Transforms")]
    private Vector3 _origin;
    public Vector3 targetLocation;

    void Awake()
    {
        _masterSpawnerScript = GameObject.Find("MasterObjectPool").GetComponent<MasterSpawner>();
        _hazardSpawnerScript = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<SpawnHazard>();
        _enemy = GetComponent<EnemyScript>();

        _minValue = 0;
        _maxValue = _masterSpawnerScript.Projectiles.Count;

        StartCoroutine(BasicAttackCoroutine());
    }

    private void Start()
    {
        StartCoroutine(SmokeCoroutine());
        _player = GameManager.Instance.Player.transform;
        _playerMovement = _player.gameObject.GetComponent<PlayerMovement>();
    }

    void Update()
    {   
        EnemyMovement();
        LevelChecker();
    }

    private void FixedUpdate()
    {
        var position = transform.position;
        position = Vector3.Lerp(position, targetLocation, Time.deltaTime * _enemyMovementSpeed);
        position = new Vector3(position.x, position.y, _origin.z);
        transform.position = position;
    }

    private IEnumerator BasicAttackCoroutine()
    {
        RandomLocation();

        yield return new WaitForSeconds(_attackCooldown);

        _spawnIndex = Random.Range(_minValue, _maxValue);

        _objectString = _spawnIndex.ToString();

        var t = transform;
        _masterSpawnerScript.SpawnProjectile(_objectString, t.position, t.rotation);

        StartCoroutine(BasicAttackCoroutine());
    }

    private Vector3 RandomLocation()
    {
        var x = (Random.Range(_minTravelDistance, _maxTravelDistance));
        var y = (Random.Range(_minTravelDistance, _maxTravelDistance));
        var z = (Random.Range(_minTravelDistance, _maxTravelDistance));

        targetLocation = _origin + new Vector3(x, y, z);

        return targetLocation;
    }

    private void LevelChecker()
    {
        if (DistanceToPlayer > 80)
        {
            MasterSpawner.playerLevel = 1;
        }

        if (DistanceToPlayer <= 80)
        {
            MasterSpawner.playerLevel = 2;
        }

        if (DistanceToPlayer <= 40)
        {
            MasterSpawner.playerLevel = 3;
        }
    }

    private void EnemyMovement()
    {
        _origin = _player.transform.position + new Vector3(0, 0, DistanceToPlayer);
        currentDistanceToNextChunk -= _playerMovement.Velocity.z * Time.deltaTime;

        if (DistanceToPlayer <= 0)
        {
            DistanceToPlayer = 0;
        }

        if (currentDistanceToNextChunk <= 0)
        {
            _hazardSpawnerScript.SpawnObjects();
            currentDistanceToNextChunk = maxDistanceToNextChunk;
        }

        _playerVelocity = _playerMovement.Velocity;
        _distanceMultiplier = _playerVelocity.z / _velocityCutoff;

        if (_playerVelocity.z > _velocityCutoff)
        {
            DistanceToPlayer -= Time.deltaTime * _distanceMultiplier;
            
        }
        else
        {
            if (DistanceToPlayer < _punishmentMaxDistance)
            {
                DistanceToPlayer += Time.deltaTime * _punishmentMoveSpeed;
            }
            else
            {
                DistanceToPlayer += Time.deltaTime * _passiveDistanceGainFromPlayer;
            }

        }

        if (DistanceToPlayer >= MaxDistanceToPlayer)
        {
            DistanceToPlayer = MaxDistanceToPlayer; 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager.InvokeOnGameWon();
        }
    }

    public float GetDistanceToPlayerPercentage()
    {
        return DistanceToPlayer / MaxDistanceToPlayer;
    }

    private IEnumerator SmokeCoroutine()
    {
        _bigSmokeParticles.Play();
        yield return new WaitForSeconds(7);
        _bigSmokeParticles.Stop();
        _smallSmokeParticles.Play();

    }
}
